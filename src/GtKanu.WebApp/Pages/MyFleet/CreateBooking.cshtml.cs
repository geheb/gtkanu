namespace GtKanu.WebApp.Pages.MyFleet;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

[Node("Fahrzeug buchen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public class CreateBookingModel : PageModel
{
    private readonly IVehicles _vehicles;

    [BindProperty]
    public CreateBookingInput Input { get; set; } = new();

    public SelectListItem[] Vehicles { get; set; } = [];

    public CreateBookingModel(IVehicles vehicles)
    {
        _vehicles = vehicles;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await UpdateView(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await UpdateView(cancellationToken)) return Page();

        var dto = Input.ToDto(User.GetId());

        if (dto.Start >= dto.End)
        {
            ModelState.AddModelError(string.Empty, "Zeitraum-Von darf nicht größer/gleich als Zeitraum-Bis sein.");
            return Page();
        }

        if (dto.Start.Date < DateTimeOffset.UtcNow.Date)
        {
            ModelState.AddModelError(string.Empty, "Zeitraum-Von darf nicht in der Vergangenheit liegen.");
            return Page();
        }

        var status = await _vehicles.CreateBooking(dto, cancellationToken);
        if (status == VehicleBookingStatus.Success)
        {
            return RedirectToPage("Index");
        }

        if (status == VehicleBookingStatus.AlreadyBooked)
        {
            ModelState.AddModelError(string.Empty, "Fahrzeug ist bereits in diesem Zeitraum gebucht.");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Speichern des Fahrzeugs.");
        }
        return Page();
    }

    private async Task<bool> UpdateView(CancellationToken cancellationToken)
    {
        var vehicles = await _vehicles.GetVehiclesInUseOnly(cancellationToken);

        var items = new List<SelectListItem> { new() };
        items.AddRange(vehicles.Select(u => new SelectListItem(u.Name, u.Id.ToString(), u.Id.ToString() == Input.VehicleId)));

        Vehicles = items.ToArray();

        return ModelState.IsValid;
    }
}
