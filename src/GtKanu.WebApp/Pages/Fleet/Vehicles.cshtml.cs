namespace GtKanu.WebApp.Pages.Fleet;

using GtKanu.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;


[Node("Fahrzeuge", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,fleetmanager")]
public class VehiclesModel : PageModel
{
    private readonly Vehicles _vehicles;

    public VehicleDto[] Items { get; set; } = Array.Empty<VehicleDto>();

    public VehiclesModel(Vehicles vehicles)
    {
        _vehicles = vehicles;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _vehicles.GetAllVehicles(cancellationToken);
    }
}
