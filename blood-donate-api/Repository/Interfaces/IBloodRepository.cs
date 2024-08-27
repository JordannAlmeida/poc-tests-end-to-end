using Domain.Model.Database;

namespace blood_donate_api.Repository.Interfaces
{
    public interface IBloodRepository
    {
        Task<IEnumerable<BloodStockModel>> GetBloodStockModelByTypeAndRhFactorAsync(string bloodType, string rhFactor);
        Task<int> RegisterDonateAsync(UserModel userModel, BloodStockModel bloodStockModel);
    }
}
