
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FCE.Configs;
using FCE.Configs.Extensions;
using FCE.Features.Common;
using FCE.Features.Metrics;
using FCE.Features.Stats.SubmitFitnessStats;
using System.Reflection;
using System.Text.Json.Serialization;

namespace FCE
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
            // app.MapFCEEndpoints();
           // app.MapSubmitCalculateMetricsEndPoint();
            app.MapSubmitFitnessStateEndPoint();
            app.Run();
        }
    }
}
