using BloodDonateApiTest.E2ETest.Abstractions;
using BloodDonateApiTest.E2ETest.Factory;
using Domain.Model.Request;
using Domain.Model.Response;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BloodDonateApiTest.E2ETest
{
    public class BloodDonateE2ETest : BaseIntegrationTest
    {
        public BloodDonateE2ETest(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        private static RegisterBloodDonateRequest SeedManoelGomes(float quantity)
        {
            return new RegisterBloodDonateRequest
            {
                BloodType = "A",
                RhFactor = "N",
                Name = "Manoel Gomes",
                UniqueCode = "13542765399",
                Age = 45,
                Quantity = quantity,
                DateDonate = DateTime.Now
            };
        }

        private static List<RegisterBloodDonateRequest> SeedListDonates()
        {
            return new() {  SeedManoelGomes(350),
                            new RegisterBloodDonateRequest
                                    {
                                        BloodType = "A",
                                        RhFactor = "N",
                                        Name = "Galvão Bueno",
                                        UniqueCode = "16542765399",
                                        Age = 70,
                                        Quantity = 200,
                                        DateDonate = DateTime.Now
                                    },
                            new RegisterBloodDonateRequest
                                    {
                                        BloodType = "A",
                                        RhFactor = "N",
                                        Name = "Silvo Santos",
                                        UniqueCode = "16542725310",
                                        Age = 70,
                                        Quantity = 400,
                                        DateDonate = DateTime.Now
                                    },
                            new RegisterBloodDonateRequest
                                    {
                                        BloodType = "B",
                                        RhFactor = "P",
                                        Name = "Fausto Silva",
                                        UniqueCode = "16542735399",
                                        Age = 70,
                                        Quantity = 250,
                                        DateDonate = DateTime.Now
                                    }

            };
        }

        [Fact]
        public async Task WhenReceiveDonate_RequestIsNotValid_ReturnBadRequest()
        {
            var request = SeedManoelGomes(0);

            var response = await Client.PostAsJsonAsync("/api/blood/register", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task WhenReceiveDonate_RequestIsValid_ReturnStatusOkWithIdOfDonate()
        {
            var request = SeedManoelGomes(350);

            var response = await Client.PostAsJsonAsync("/api/blood/register", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(await response.Content.ReadAsStringAsync());
            Assert.True(int.TryParse(await response.Content.ReadAsStringAsync(), out _));
        }

        [Fact]
        public async Task WhenRequestStockOfBlood_AndParametersAreInvalid_ReturnBadRequest()
        {
            var bloodType = "A";

            var response = await Client.GetAsync($"/api/blood/stock?bloodType={bloodType}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("\"bloodType and rhFactor must be sent\"", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task WhenRequestStockOfBlood_AndParametersAreValid_AndItWasMakeDonates_ReturnOkWithTotalStock()
        {
            var listRequestDonates = SeedListDonates();

            foreach (RegisterBloodDonateRequest request in listRequestDonates)
            {
                await Client.PostAsJsonAsync("/api/blood/register", request);
            }

            var bloodType = "A";
            var rhFactor = "N";
            var response = await Client.GetAsync($"/api/blood/stock?bloodType={bloodType}&rhFactor={rhFactor}");
            var totalStockDonated = listRequestDonates.Where(x => x.BloodType == bloodType && x.RhFactor == rhFactor).Sum(x => x.Quantity);
            var totalStock = await response.Content.ReadFromJsonAsync<BloodStockResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(totalStockDonated, totalStock.Stock);
        }

        [Fact]
        public async Task WhenRequestStockOfBlood_AndParametersAreValid_AndItWasMakeDonates_AndMakeSearchTwoTimes_ReturnOkWithTotalStock()
        {
            var listRequestDonates = SeedListDonates();

            foreach (RegisterBloodDonateRequest request in listRequestDonates)
            {
                var requestHttp = new HttpRequestMessage(HttpMethod.Post, "/api/blood/register");
                requestHttp.Headers.Authorization = new AuthenticationHeaderValue("Test");
                await Client.SendAsync(requestHttp);
            }

            var bloodType = "A";
            var rhFactor = "N";
            var totalStockDonated = listRequestDonates.Where(x => x.BloodType == bloodType && x.RhFactor == rhFactor).Sum(x => x.Quantity);

            var requestHttpWithoutCache = new HttpRequestMessage(HttpMethod.Get, $"/api/blood/stock?bloodType={bloodType}&rhFactor={rhFactor}");
            requestHttpWithoutCache.Headers.Authorization = new AuthenticationHeaderValue("Test");
            var responseWithoutCache = await Client.SendAsync(requestHttpWithoutCache);
            var totalStockWithoutCache = await responseWithoutCache.Content.ReadFromJsonAsync<BloodStockResponse>();
            
            var requestHttpWithCache = new HttpRequestMessage(HttpMethod.Get, $"/api/blood/stock?bloodType={bloodType}&rhFactor={rhFactor}");
            requestHttpWithCache.Headers.Authorization = new AuthenticationHeaderValue("Test");
            var responseWithCache = await Client.SendAsync(requestHttpWithCache);
            var totalStockWitCache = await responseWithCache.Content.ReadFromJsonAsync<BloodStockResponse>();

            Assert.Equal(HttpStatusCode.OK, responseWithoutCache.StatusCode);
            Assert.Equal(totalStockDonated, totalStockWithoutCache.Stock);
            Assert.Equal(HttpStatusCode.OK, responseWithCache.StatusCode);
            Assert.Equal(totalStockDonated, totalStockWitCache.Stock);
            Assert.Equal(totalStockWithoutCache.Stock, totalStockWitCache.Stock);
        }
    }
}