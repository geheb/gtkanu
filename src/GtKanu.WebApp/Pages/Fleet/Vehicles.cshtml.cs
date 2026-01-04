namespace GtKanu.WebApp.Pages.Fleet;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;


[Node("Fahrzeuge", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,fleetmanager")]
public class VehiclesModel : PageModel
{
    private readonly IVehicles _vehicles;

    public VehicleDto[] Items { get; set; } = [];

    public VehiclesModel(IVehicles vehicles)
    {
        _vehicles = vehicles;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _vehicles.GetAllVehicles(cancellationToken);
    }
}
