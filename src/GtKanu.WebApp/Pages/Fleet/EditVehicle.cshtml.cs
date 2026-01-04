namespace GtKanu.WebApp.Pages.Fleet;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Fahrzeug bearbeiten", FromPage = typeof(VehiclesModel))]
[Authorize(Roles = "administrator,fleetmanager")]
public class EditVehicleModel : PageModel
{
    private readonly IVehicles _vehicles;

    [BindProperty]
    public VehicleInput Input { get; set; } = new();

    public bool IsDisabled { get; set; }

    public EditVehicleModel(IVehicles vehicles)
    {
        _vehicles = vehicles;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _vehicles.FindVehicle(id, cancellationToken);
        if (dto is null)
        {
            ModelState.AddModelError(string.Empty, "Fahrzeug wurde nicht gefunden.");
            IsDisabled = true;
            return;
        }

        Input.From(dto);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _vehicles.FindVehicle(id, cancellationToken);
        if (dto is null)
        {
            ModelState.AddModelError(string.Empty, "Fahrzeug wurde nicht gefunden.");
            IsDisabled = true;
            return Page();
        }

        Input.To(dto);

        var result = await _vehicles.UpdateVehicle(dto, cancellationToken);
        if (result != VehicleStatus.Success)
        {
            if (result == VehicleStatus.Exists)
            {
                ModelState.AddModelError(string.Empty, "Fahrzeug existiert bereits.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Fehler beim Speichern des Fahrzeugs.");
            }
            return Page();
        }

        return RedirectToPage("Vehicles");
    }
}
