using blood_donate_report_api.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blood_donate_report_api.Controllers
{
    public static class BloodReportControllerExtension
    {
        public static void RegisterBloodReportController(this WebApplication app)
        {
            var bloodController = app.MapGroup("api/blood-report");
            bloodController.MapGet("/stock/getReportOfAll", [Authorize] async ([FromServices] IBloodReportService bloodService) =>
            {
                try
                {
                    var result = await bloodService.GetStockOfAll();
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error in get endpoint /stock/getReportOfAll");
                    return Results.Problem("Error to get Report", "BloodController", 500);
                }
            }).RequireAuthorization();
        }
    }
}
