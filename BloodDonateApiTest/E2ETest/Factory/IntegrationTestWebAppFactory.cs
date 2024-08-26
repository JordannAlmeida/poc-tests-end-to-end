using BlondDonateContainerTests.Factory;
using blood_donate_api;
using blood_donate_api.Repository;
using blood_donate_api.Repository.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace BlondDonateApiTest.E2ETest.Factory
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();
        private readonly RedisContainer _redisContainer = new RedisBuilder().Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _dbContainer.InitializeDatabaseTestContainerAsync().Wait();

            builder.ConfigureTestServices(services =>
            {
                ReplaceConnectionStringRedis(services);
                ReplaceConnectionStringSqlServer(services);
            });
        }

        public async Task ResetData()
        {
            await Task.WhenAll(new List<Task>() { 
                _dbContainer.ResetData(),
                _redisContainer.ExecScriptAsync("FLUSHALL")
            });
        }

        private void ReplaceConnectionStringRedis(IServiceCollection services)
        {
            var descriptorTypeRedisCache =
                    typeof(IDistributedCache);

            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == descriptorTypeRedisCache);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = _redisContainer.GetConnectionString();
                });
            }
        }

        private void ReplaceConnectionStringSqlServer(IServiceCollection services)
        {
            var descriptorTypeDbSqlServerBloodRepository =
                    typeof(IBloodRepository);

            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == descriptorTypeDbSqlServerBloodRepository);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
                services.AddSingleton<IBloodRepository>(provider => new BloodRepository(_dbContainer.GetConnectionString()));
            }
        }

        public Task InitializeAsync()
        {
            _redisContainer.StartAsync().Wait();
            return _dbContainer.StartAsync();
        }

        public new Task DisposeAsync()
        {
            _redisContainer.StopAsync().Wait();
            return _dbContainer.StopAsync();
        }
    }
}
