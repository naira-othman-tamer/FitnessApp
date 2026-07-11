using Autofac;
using Autofac.Extensions.DependencyInjection;
using FCE.Configs;
using FCE.Configs.Extensions;
using FCE.Features.Common;
using MassTransit;
using System.Reflection;
using System.Text.Json.Serialization;
using ContractMessages.NutritionPlanMatching;
using ContractMessages.WorkoutPlanMatching;
using FCE.Integrations.Consumers;
using Microsoft.OpenApi.Models;

namespace FCE
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddDatabase(builder.Configuration);
            builder.Services.AddMediatR(cfg =>
                 cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
                containerBuilder.RegisterModule(new AutofacModule()));

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(
                    new JsonStringEnumConverter());
            });

            builder.Services.AddMassTransit(x =>
            {
                
                x.AddRequestClient<IGetWorkoutPlanRequest>();
                x.AddRequestClient<IGetNutritionPlanRequest>(new Uri("queue:nutrition-plan-matching"));
                x.AddConsumer<UserMetricsConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "localhost";
                    cfg.Host($"rabbitmq://{rabbitHost}", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("fce-user-metrics", e =>
                    {
                        e.ConfigureConsumer<UserMetricsConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            //builder.Services.AddScoped<IRequestClient<IGetWorkoutPlanRequest>>(sp =>
            //        sp.GetRequiredService<IBus>().CreateRequestClient<IGetWorkoutPlanRequest>());

            var app = builder.Build();

            await app.MigrateDatabaseAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers().RequireAuthorization();
            app.MapFCEEndpoints();
           
            app.Run();
        }
    }
}
