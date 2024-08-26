using BlondDonateApiTest.E2ETest.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace BlondDonateApiTest.E2ETest.Abstractions
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
    {
        protected HttpClient Client { get; init; }

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            factory.ResetData().Wait();
            Client = factory.CreateClient();
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
