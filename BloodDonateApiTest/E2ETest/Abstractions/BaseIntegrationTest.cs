using BloodDonateApiTest.E2ETest.Factory;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BloodDonateApiTest.E2ETest.Abstractions
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
    {
        protected HttpClient Client { get; init; }

        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
            factory.ResetData().Wait();
            Client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
