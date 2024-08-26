using blood_donate_api.Models.Requests;
using blood_donate_api.Models.Responses;
using System.Text.Json.Serialization;

namespace blood_donate_api.Models.SourceGenerator
{
    [JsonSerializable(typeof(IEnumerable<RegisterBloodDonateRequest>))]
    [JsonSerializable(typeof(IEnumerable<BloodStockResponse>))]
    [JsonSerializable(typeof(IEnumerable<BloodStockModel>))]
    [JsonSerializable(typeof(RegisterBloodDonateRequest))]
    [JsonSerializable(typeof(BloodStockResponse))]
    [JsonSerializable(typeof(BloodStockModel))]
    [JsonSerializable(typeof(UserModel))]
    [JsonSerializable(typeof(DateTime))]
    public partial class AppJsonSerializerContext : JsonSerializerContext
    {

    }
}
