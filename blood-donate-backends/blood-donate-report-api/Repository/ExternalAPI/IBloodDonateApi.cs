using Domain.Model.Response;

namespace blood_donate_report_api.Repository.ExternalAPI
{
    public interface IBloodDonateApi
    {
        Task<BloodStockResponse?> GetStock(string bloodType, string rhFactor);
    }
}
