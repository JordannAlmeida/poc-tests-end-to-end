using Domain.Model.Response;

namespace blood_donate_report_api.Service.Interfaces
{
    public interface IBloodReportService
    {
        Task<IEnumerable<BloodReportStockResponse?>> GetStockOfAll();
    }
}
