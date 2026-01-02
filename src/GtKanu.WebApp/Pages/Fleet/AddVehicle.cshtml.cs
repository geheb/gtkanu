namespace GtKanu.WebApp.Pages.Fleet;

using GtKanu.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Fahrzeug anlegen", FromPage = typeof(VehiclesModel))]
[Authorize(Roles = "administrator,fleetmanager")]
public class AddVehicleModel : PageModel
{
    private readonly Vehicles _vehicles;

    [BindProperty]
    public VehicleInput Input { get; set; } = new();

    public AddVehicleModel(Vehicles vehicles)
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
                ModelState.AddModelError(string.Empty, I18n.LocalizedMessages.ItemExists);
            }
            else
            {
                ModelState.AddModelError(string.Empty, I18n.LocalizedMessages.ItemCreateFailed);
            }

            return Page();
        }

        return RedirectToPage("Vehicles");
    }
}
