using blood_donate_api.Controllers;
using blood_donate_api.Extensions;
using blood_donate_api.Repository;
using blood_donate_api.Repository.Interfaces;
using blood_donate_api.Services;
using blood_donate_api.Services.Interfaces;
using Domain.SourceGenerator;
using Serilog;

namespace blood_donate_api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Log.Logger = LogExtensions.CreateInitialLog();

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            });
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
            });
            builder.Services.AddSingleton<IBloodRepository>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<BloodRepository>>();
                var connectionString = builder.Configuration.GetConnectionString("SqlServer");
                return new BloodRepository(connectionString ?? "", logger);
            });
            builder.Services.AddSingleton<ICacheRepository, CacheRepository>();
            builder.Services.AddSingleton(new AppJsonSerializerContext());
            builder.Services.AddScoped<IBloodService, BloodService>();
            builder.Services.AddCustomAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Host.UseSerilog(LogExtensions.ConfigureLogger);
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwagger();
            }
            app.UseHttpsRedirection();
            app.RegisterBloodController();
            
            app.Run();
        }
    }
}
