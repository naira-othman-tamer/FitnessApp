
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FCE.Features.Common;
using MassTransit;
using System.Reflection;
using System.Text.Json.Serialization;
using WorkoutService.Configs;
using WorkoutService.Configs.Extensions;
using WorkoutService.Consumers;

namespace WorkoutService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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



            #region MassTransit Configuration 
            builder.Services.AddMassTransit(x =>
               {
                   // Register the consumer that handles the workout plan matching request
                   x.AddConsumer<WorkoutPlanMatchingConsumer>();

                   x.UsingRabbitMq((context, cfg) =>
                   {
                       cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
                       {
                           h.Username(builder.Configuration["RabbitMq:Username"] ?? "guest");
                           h.Password(builder.Configuration["RabbitMq:Password"] ?? "guest");
                       });

                       // Dedicated receive endpoint/queue for plan matching requests
                       cfg.ReceiveEndpoint("workout-plan-matching", e =>
                       {
                           e.ConfigureConsumer<WorkoutPlanMatchingConsumer>(context);

                           // Optional: retry policy if consumer logic throws
                           e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));
                       });

                       // Auto-configures any other consumers registered above
                       cfg.ConfigureEndpoints(context);
                   });
               });

            #endregion
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
            app.MapWorkoutEndpoints();
            app.Run();
        }
    }
}
