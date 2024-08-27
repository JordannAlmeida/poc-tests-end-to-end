using Serilog;
using Serilog.Core;
using Serilog.Extensions.Hosting;
using Serilog.Sinks.Elasticsearch;

namespace blood_donate_api.Extensions
{
    public static class LogExtensions
    {
        public static ReloadableLogger CreateInitialLog()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .CreateBootstrapLogger();
        }

        public static void ConfigureLogger(HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration)
        {
            IConfiguration config = context.Configuration;
            LoggingLevelSwitch levelSwitch = services.GetService<LoggingLevelSwitch>() ?? new LoggingLevelSwitch();

            configuration
                .ReadFrom.Services(services)
                .MinimumLevel.ControlledBy(levelSwitch)
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(config.GetSection("ElasticConfiguration")["uri"] ?? ""))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    TypeName = null,
                    BatchAction = ElasticOpType.Create,
                    IndexFormat = "blood-donate-api-{0:yyyy.MM.dd}"
                });
        }
    }
}
