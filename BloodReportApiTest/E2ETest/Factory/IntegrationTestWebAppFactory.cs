using blood_donate_report_api;
using BloodReportApiTest.E2ETest.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Elasticsearch;
using WireMock.Server;

namespace BloodReportApiTest.E2ETest.Factory
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly int portWireMock = 9876;
        private readonly ElasticsearchContainer _elasticsearch = new ElasticsearchBuilder().Build();
        private WireMockServer? _server;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("local");
            builder.ConfigureTestServices(services => {
                ReplaceConfigVariables(services);
                RemoveAuthentication(services);
            });
        }

        private void ReplaceConfigVariables(IServiceCollection services)
        {
            IConfiguration? configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            if (configuration is not null)
            {
                configuration["ElasticConfiguration:uri"] = _elasticsearch.GetConnectionString();
                configuration["BloodDonateApi:baseAdress"] = $"{_server.Urls[0]}/";
            }
        }

        private static void RemoveAuthentication(IServiceCollection services)
        {
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
        }

        public WireMockServer GetWireMockServer => _server ?? throw new InvalidOperationException("Server is not initialized");

        public Task InitializeAsync()
        {
            _server = WireMockServer.Start(portWireMock);
            return _elasticsearch.StartAsync();
        }

        public new Task DisposeAsync()
        {
            _server?.Stop();
            _server?.Dispose();
            return _elasticsearch.StopAsync();
        }
    }
}
