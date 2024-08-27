using Domain.Model.Response;
using System.Diagnostics;
using System.Text.Json;

namespace blood_donate_report_api.Repository.ExternalAPI
{
    public class BloodDonateApi(IHttpClientFactory httpClientFactory, ILogger<BloodDonateApi> logger) : IBloodDonateApi
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("BloodDonateApi");
        private readonly ILogger<BloodDonateApi> _logger = logger;

        public async Task<BloodStockResponse?> GetStock(string bloodType, string rhFactor)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            var response = await _httpClient.GetAsync($"api/blood/stock?bloodType={bloodType}&rhFactor={rhFactor}");
            response.EnsureSuccessStatusCode();
            stopwatch.Stop();
            long timeInMilisseconds = stopwatch.ElapsedMilliseconds;
            _logger.LogInformation($"Time to get stock in ({_httpClient.BaseAddress}api/blood/stock?bloodType={bloodType}&rhFactor={rhFactor}) was {timeInMilisseconds} ms");
            return await response.Content.ReadFromJsonAsync<BloodStockResponse>();
        }
    }
}
