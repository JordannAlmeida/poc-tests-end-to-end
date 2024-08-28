using BloodReportApiTest.E2ETest.Factory;
using Microsoft.AspNetCore.Mvc.Testing;
using WireMock.Server;

namespace BloodReportApiTest.E2ETest.Abstractions
{
    public abstract class BaseIntegrationTest(IntegrationTestWebAppFactory factory) : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
    {
        protected HttpClient Client { get; init; } = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        protected WireMockServer _Server { get; init; } = factory.GetWireMockServer;

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
