using blood_donate_api.Models.Requests;
using blood_donate_api.Models.Responses;

namespace blood_donate_api.Services.Interfaces
{
    public interface IBloodService
    {
        Task<BloodStockResponse> GetStockResponseAsync(string bloodType, string rhFactor);
        Task<int> RegisterDonate(RegisterBloodDonateRequest request);
    }
}
