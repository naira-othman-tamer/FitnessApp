using Autofac;
using Autofac.Extensions.DependencyInjection;
using FCE.Configs;
using FCE.Configs.Extensions;
using FCE.Features.Common;
using MassTransit;
using System.Reflection;
using System.Text.Json.Serialization;
using ContractMessages.WorkoutPlanMatching;
using FCE.Integrations.Consumers;

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
            builder.Services.AddSwaggerGen();
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
            app.UseAuthorization();
            app.MapControllers();
            app.MapFCEEndpoints();
           
            app.Run();
        }
    }
}
