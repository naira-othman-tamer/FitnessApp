
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using ProgressTrackingService.Configs.Extensions;
using ProgressTrackingService.Features.Common;
using System.Reflection;
using System.Text.Json.Serialization;
using WorkoutService.Configs;

namespace ProgressTrackingService
{
    public class Program
    {
        public static void Main(string[] args)
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

            builder.Services.AddMassTransit(x =>
            {

                //x.AddRequestClient<IGetWorkoutPlanRequest>();
                //x.AddRequestClient<IGetNutritionPlanRequest>(new Uri("queue:nutrition-plan-matching"));
                //x.AddConsumer<UserMetricsConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "localhost";
                    cfg.Host($"rabbitmq://{rabbitHost}", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("", e =>
                    {
                        //e.ConfigureConsumer<UserMetricsConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.MapProgressServiceEndPoints();
            app.Run();
        }
    }
}

