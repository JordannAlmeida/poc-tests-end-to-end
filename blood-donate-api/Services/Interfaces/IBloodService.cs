using Domain.Model.Request;
using Domain.Model.Response;

namespace blood_donate_api.Services.Interfaces
{
    public interface IBloodService
    {
        Task<BloodStockResponse> GetStockResponseAsync(string bloodType, string rhFactor);
        Task<int> RegisterDonate(RegisterBloodDonateRequest request);
    }
}
