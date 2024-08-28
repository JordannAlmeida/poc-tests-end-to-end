using BlondDonateContainerTests.Factory;
using blood_donate_api;
using BloodDonateApiTest.E2ETest.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Elasticsearch;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace BloodDonateApiTest.E2ETest.Factory
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();
        private readonly ElasticsearchContainer _elasticsearch = new ElasticsearchBuilder().Build();
        private readonly RedisContainer _redisContainer = new RedisBuilder().Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _dbContainer.InitializeDatabaseTestContainerAsync().Wait();
            builder.UseEnvironment("local");
            builder.ConfigureTestServices(services => {
                ReplaceConfigVariables(services);
                RemoveAuthentication(services);
            });
        }

        public async Task ResetData()
        {
            await Task.WhenAll(new List<Task>() {
                _dbContainer.ResetData(),
                _redisContainer.ExecScriptAsync("FLUSHALL")
            });
        }

        private void ReplaceConfigVariables(IServiceCollection services)
        {
            IConfiguration? configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            if (configuration is not null)
            {
                configuration["ElasticConfiguration:uri"] = _elasticsearch.GetConnectionString();
                configuration["ConnectionStrings:SqlServer"] = _dbContainer.GetConnectionString();
                configuration["ConnectionStrings:Redis"] = _redisContainer.GetConnectionString();
            }
        }

        private static void RemoveAuthentication(IServiceCollection services)
        {
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
        }

        public Task InitializeAsync()
        {
            _redisContainer.StartAsync().Wait();
            _elasticsearch.StartAsync().Wait();
            return _dbContainer.StartAsync();
        }

        public new Task DisposeAsync()
        {
            _redisContainer.StopAsync().Wait();
            _elasticsearch.StopAsync().Wait();
            return _dbContainer.StopAsync();
        }
    }
}
