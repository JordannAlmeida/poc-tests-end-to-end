using BloodReportApiTest.E2ETest.Abstractions;
using BloodReportApiTest.E2ETest.Factory;
using Domain.Model.Response;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace BloodReportApiTest.E2ETest
{
    public class BloodReportE2ETest(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
    {
        private readonly string[] _bloodTypes = { "A", "B", "AB", "O" };
        private readonly string[] _rhFactors = { "P", "N" };

        private void CreateResponseSuccessForGetStockOfBloodDonateApi(string bloodType, string rhFactor)
        {
            BloodStockResponse bloodStockResponse = new()
            {
                BloodType = bloodType,
                RhFactor = rhFactor,
                Stock = 100.0D
            };

            _Server.Given(
                Request.Create().WithPath($"/api/blood/stock")
                .WithParam("bloodType", bloodType)
                .WithParam("rhFactor", rhFactor)
                .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonSerializer.Serialize(bloodStockResponse))
            );
        }

        private void CreateResponseErrorIfBloodTypeEqualsAForGetStockOfBloodDonateApi(string bloodType, string rhFactor)
        {
            BloodStockResponse bloodStockResponse = new()
            {
                BloodType = bloodType,
                RhFactor = rhFactor,
                Stock = 100.0D
            };

            if(bloodType.Equals("A"))
            {
                _Server.Given(
                    Request.Create()
                    .WithPath($"/api/blood/stock")
                    .WithParam("bloodType", bloodType)
                    .WithParam("rhFactor", rhFactor)
                    .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(500)
                        .WithHeader("Content-Type", "text/plain")
                        .WithBody("Error on server")
                );
            }
            else
            {
                _Server.Given(
                    Request.Create().WithUrl($"/api/blood/stock")
                    .WithParam("bloodType", bloodType)
                    .WithParam("rhFactor", rhFactor)
                    .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(JsonSerializer.Serialize(bloodStockResponse))
                );
            }
        }

        private void PopulateServerWiremockWithSuccessResponses()
        {
            foreach (var type in _bloodTypes)
            {
                foreach (var factor in _rhFactors)
                {
                    CreateResponseSuccessForGetStockOfBloodDonateApi(type, factor);
                }
            }
        }

        private void PopulateServerWiremockWithErrorResponses()
        {
            foreach (var type in _bloodTypes)
            {
                foreach (var factor in _rhFactors)
                {
                    CreateResponseErrorIfBloodTypeEqualsAForGetStockOfBloodDonateApi(type, factor);
                }
            }
        }

        [Fact]
        public async Task WhenCallEndPointGetReportOfAll_AndBoodDonateApiReturnError_ReturnInternalServerError()
        {
            PopulateServerWiremockWithErrorResponses();
            var response = await Client.GetAsync($"api/blood-report/stock/getReportOfAll");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task WhenCallEndPointGetReportOfAll_AndBoodDonateApiReturnSuccess_ReturnListOfStockByEachTypeOfBloodAndRhFactor()
        {
            PopulateServerWiremockWithSuccessResponses();
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/blood-report/stock/getReportOfAll");
            request.Headers.Authorization = new AuthenticationHeaderValue("Test");
            var response = await Client.SendAsync(request);

            IEnumerable<BloodReportStockResponse> bloodStockResponses = await response.Content.ReadFromJsonAsync<IEnumerable<BloodReportStockResponse>>() ?? [];

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(8, bloodStockResponses.Count());
            Assert.Equal(800D, bloodStockResponses.Select(x => x.TotalQuantity).Sum());
            foreach (var type in _bloodTypes)
            {
                foreach (var factor in _rhFactors)
                {
                    Assert.Equal(100D, bloodStockResponses.Where(x => x.BloodType.Equals(type) && x.RhFactor.Equals(factor)).FirstOrDefault().TotalQuantity);
                }
            }
        }
    }
}