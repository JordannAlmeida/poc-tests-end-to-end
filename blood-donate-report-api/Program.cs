
using blood_donate_report_api.Controllers;
using blood_donate_report_api.Extensions;
using blood_donate_report_api.Repository.ExternalAPI;
using blood_donate_report_api.Service;
using blood_donate_report_api.Service.Interfaces;
using Domain.SourceGenerator;
using Serilog;

namespace blood_donate_report_api
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
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var configuration = builder.Configuration;

            builder.Services.AddHttpClient("BloodDonateApi", (client) =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("BloodDonateApi")["baseAdress"] ?? throw new ArgumentNullException("BloodDonateApi:baseAdress", "must be set in environment variables"));
            });
            builder.Services.AddSingleton<IBloodDonateApi, BloodDonateApi>();
            builder.Services.AddScoped<IBloodReportService, BloodReportService>();

            builder.Host.UseSerilog(LogExtensions.ConfigureLogger);
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.RegisterBloodReportController();

            app.Run();
        }
    }
}
