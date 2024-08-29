using Domain.Model.Database;
using Domain.Model.Request;
using Domain.Model.Response;
using System.Text.Json.Serialization;

namespace Domain.SourceGenerator
{
    [JsonSerializable(typeof(IEnumerable<RegisterBloodDonateRequest>))]
    [JsonSerializable(typeof(IEnumerable<BloodStockResponse>))]
    [JsonSerializable(typeof(IEnumerable<BloodStockModel>))]
    [JsonSerializable(typeof(IEnumerable<BloodReportStockResponse>))]
    [JsonSerializable(typeof(BloodReportStockResponse))]
    [JsonSerializable(typeof(RegisterBloodDonateRequest))]
    [JsonSerializable(typeof(BloodStockResponse[]))]
    [JsonSerializable(typeof(BloodStockResponse?))]
    [JsonSerializable(typeof(BloodStockResponse))]
    [JsonSerializable(typeof(BloodStockModel))]
    [JsonSerializable(typeof(UserModel))]
    [JsonSerializable(typeof(DateTime))]
    public partial class AppJsonSerializerContext : JsonSerializerContext
    {

    }
}
