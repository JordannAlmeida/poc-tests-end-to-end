using blood_donate_report_api.Repository.ExternalAPI;
using blood_donate_report_api.Service.Interfaces;
using Domain.Model.Response;

namespace blood_donate_report_api.Service
{
    public class BloodReportService(IBloodDonateApi bloodDonateApi, ILogger<BloodReportService> logger) : IBloodReportService
    {
        private readonly IBloodDonateApi _bloodDonateApi = bloodDonateApi;
        private readonly ILogger<BloodReportService> _logger = logger;
        private readonly string[] _bloodTypes = { "A", "B", "AB", "O" };
        private readonly string[] _rhFactors = { "+", "-" };

        public async Task<IEnumerable<BloodReportStockResponse?>> GetStockOfAll()
        {
            List<Task<BloodStockResponse?>> tasks = new();
            foreach (var type in _bloodTypes)
            {
                foreach (var factor in _rhFactors)
                {
                    tasks.Add(_bloodDonateApi.GetStock(type, factor));
                }
            }
            var responses = await Task.WhenAll(tasks);
            return responses.Select(response => new BloodReportStockResponse(response.Value.BloodType, response.Value.RhFactor, response.Value.Stock)).ToList();
        }
    }
}
