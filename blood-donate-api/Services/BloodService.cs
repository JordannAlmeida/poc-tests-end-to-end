using blood_donate_api.Models;
using blood_donate_api.Models.Requests;
using blood_donate_api.Models.Responses;
using blood_donate_api.Models.SourceGenerator;
using blood_donate_api.Repository.Interfaces;
using blood_donate_api.Services.Interfaces;

namespace blood_donate_api.Services
{
    public class BloodService(IBloodRepository bloodRepository, ICacheRepository cacheRepository, AppJsonSerializerContext appJsonSerializerContext) : IBloodService
    {
        private readonly IBloodRepository _bloodRepository = bloodRepository;
        private readonly ICacheRepository _cacheRepository = cacheRepository;
        private readonly AppJsonSerializerContext _appJsonSerializerContext = appJsonSerializerContext;
        private readonly int _lifeTimeMinutes = 5;

        public async Task<BloodStockResponse> GetStockResponseAsync(string bloodType, string rhFactor)
        {
            var keyCache = $"{bloodType}_{rhFactor}";
            var listFromCache = await _cacheRepository.TryGetValueAsync<IEnumerable<BloodStockModel>>(keyCache, _appJsonSerializerContext.Options);
            if (listFromCache != null)
            {
                return new BloodStockResponse { Stock = CalculateStock(listFromCache) };
            }
            var listBloodDonateByType = await _bloodRepository.GetBloodStockModelByTypeAndRhFactorAsync(bloodType, rhFactor);
            await _cacheRepository.TrySetValueAsync(keyCache, listBloodDonateByType, _lifeTimeMinutes, _appJsonSerializerContext.Options);
            return new BloodStockResponse { Stock = CalculateStock(listBloodDonateByType) };
        }

        public async Task<int> RegisterDonate(RegisterBloodDonateRequest request)
        {
            UserModel userModel = new (request.Name, request.Age, request.UniqueCode);
            BloodStockModel bloodStockModel = new(request.BloodType, request.RhFactor, request.Quantity, request.DateDonate);
            int donateId = await _bloodRepository.RegisterDonateAsync(userModel,bloodStockModel);
            return donateId;
        }

        private double CalculateStock(IEnumerable<BloodStockModel> listBloodDonateByType)
        {
            return listBloodDonateByType.Aggregate(0D, (total, curr) => total + curr.Quantity);
        }
    }
}
