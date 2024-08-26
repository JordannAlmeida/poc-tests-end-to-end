using blood_donate_api.Controllers;
using blood_donate_api.Models.SourceGenerator;
using blood_donate_api.Repository;
using blood_donate_api.Repository.Interfaces;
using blood_donate_api.Services;
using blood_donate_api.Services.Interfaces;

namespace blood_donate_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            });
            builder.Services.AddSingleton<IBloodRepository>(provider =>
            {
                var connectionString = builder.Configuration.GetConnectionString("SqlServer");
                return new BloodRepository(connectionString ?? "");
            });
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
            });
            builder.Services.AddSingleton<ICacheRepository, CacheRepository>();
            builder.Services.AddSingleton(new AppJsonSerializerContext());
            builder.Services.AddScoped<IBloodService, BloodService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.MapControllers();
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
