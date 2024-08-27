using blood_donate_api.Services.Interfaces;
using Domain.Model.Request;
using Microsoft.AspNetCore.Mvc;

namespace blood_donate_api.Controllers
{
    public static class BloodControllerExtension
    {
        public static void RegisterBloodController(this WebApplication app)
        {

            var bloodController = app.MapGroup("api/blood");
            bloodController.MapPost("/register", async ([FromBody] RegisterBloodDonateRequest request, [FromServices] IBloodService bloodService) =>
            {
                if(!request.IsValid())
                {
                    return Results.BadRequest();
                }
                try
                {
                    var result = await bloodService.RegisterDonate(request);
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Error in post endpoint api/blood/register");
                    return Results.Problem("Error to register new donate", "BloodController", 500);

                }
            });

            bloodController.MapGet("/stock", async (string? bloodType, string? rhFactor, [FromServices] IBloodService bloodService) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(bloodType) || string.IsNullOrEmpty(rhFactor)) {
                        return Results.BadRequest("bloodType and rhFactor must be sent");
                    } 
                    var result = await bloodService.GetStockResponseAsync(bloodType, rhFactor);
                    return Results.Ok(result);
               } 
               catch (Exception ex)
               {
                    app.Logger.LogError(ex, $"Error in get endpoint api/blood/stock?bloodType={bloodType}&rhFactor={rhFactor}");
                    return Results.Problem("Error to get Stock", "BloodController", 500);
               }
            });
        }
    }
}
