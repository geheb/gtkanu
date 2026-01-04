namespace GtKanu.WebApp.Pages.Fleet;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Fahrzeug anlegen", FromPage = typeof(VehiclesModel))]
[Authorize(Roles = "administrator,fleetmanager")]
public class AddVehicleModel : PageModel
{
    private readonly IVehicles _vehicles;

    [BindProperty]
    public VehicleInput Input { get; set; } = new();

    public AddVehicleModel(IVehicles vehicles)
    {
        _vehicles = vehicles;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var dto = new VehicleDto();
        Input.To(dto);

        var result = await _vehicles.CreateVehicle(dto, cancellationToken);
        if (result != VehicleStatus.Success)
        {
            if (result == VehicleStatus.Exists)
            {
                ModelState.AddModelError(string.Empty, "Fahrzeug existiert bereits");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Fehler beim Anlegen des Fahrzeugs");
            }

            return Page();
        }

        return RedirectToPage("Vehicles");
    }
}
