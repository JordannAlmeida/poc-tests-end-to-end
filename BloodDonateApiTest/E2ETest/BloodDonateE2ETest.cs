using BlondDonateApiTest.E2ETest.Abstractions;
using BlondDonateApiTest.E2ETest.Factory;
using blood_donate_api.Models.Requests;
using blood_donate_api.Models.Responses;
using System.Net;
using System.Net.Http.Json;

namespace BlondDonateApiTest.E2ETest
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
                RhFactor = "-",
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
                                        RhFactor = "-",
                                        Name = "Galvão Bueno",
                                        UniqueCode = "16542765399",
                                        Age = 70,
                                        Quantity = 200,
                                        DateDonate = DateTime.Now
                                    },
                            new RegisterBloodDonateRequest
                                    {
                                        BloodType = "A",
                                        RhFactor = "-",
                                        Name = "Silvo Santos",
                                        UniqueCode = "16542725310",
                                        Age = 70,
                                        Quantity = 400,
                                        DateDonate = DateTime.Now
                                    },
                            new RegisterBloodDonateRequest
                                    {
                                        BloodType = "B",
                                        RhFactor = "+",
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
            var rhFactor = "-";
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
                await Client.PostAsJsonAsync("/api/blood/register", request);
            }

            var bloodType = "A";
            var rhFactor = "-";
            var totalStockDonated = listRequestDonates.Where(x => x.BloodType == bloodType && x.RhFactor == rhFactor).Sum(x => x.Quantity);

            var responseWithoutCache = await Client.GetAsync($"/api/blood/stock?bloodType={bloodType}&rhFactor={rhFactor}");
            var totalStockWithoutCache = await responseWithoutCache.Content.ReadFromJsonAsync<BloodStockResponse>();

            var responseWithCache = await Client.GetAsync($"/api/blood/stock?bloodType={bloodType}&rhFactor={rhFactor}");
            var totalStockWitCache = await responseWithCache.Content.ReadFromJsonAsync<BloodStockResponse>();

            Assert.Equal(HttpStatusCode.OK, responseWithoutCache.StatusCode);
            Assert.Equal(totalStockDonated, totalStockWithoutCache.Stock);
            Assert.Equal(HttpStatusCode.OK, responseWithCache.StatusCode);
            Assert.Equal(totalStockDonated, totalStockWitCache.Stock);
            Assert.Equal(totalStockWithoutCache.Stock, totalStockWitCache.Stock);
        }
    }
}