namespace GtKanu.Ui.Pages.Trips;

using GtKanu.Core.Models;
using GtKanu.Core.Repositories;
using GtKanu.Core.User;
using GtKanu.Ui.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

[Node("Fahrt bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,tripmanager")]
public class EditTripModel : PageModel
{
    private readonly IdentityRepository _identityRepository;
    private readonly Trips _trips;

    [BindProperty]
    public TripInput Input { get; set; } = new TripInput();

    public string? Details { get; private set; }
    public bool IsDisabled { get; set; }

    public SelectListItem[] Users { get; set; } = Array.Empty<SelectListItem>();

    public EditTripModel(
        IdentityRepository identityRepository, 
        Trips trips)
    {
        _identityRepository = identityRepository;
        _trips = trips;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var trip = await UpdateView(id, cancellationToken);
        if (trip == null) return;
        Input.From(trip);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        if (await UpdateView(id, cancellationToken) == null) return Page();

        var error = Input.Validate();
        if (!string.IsNullOrEmpty(error))
        {
            ModelState.AddModelError(string.Empty, error);
            return Page();
        }

        var dto = Input.ToDto(id);

        var result = await _trips.UpdateTrip(dto, cancellationToken);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Anlegen der Fahrt.");
            return Page();
        }

        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostDeleteTripAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _trips.DeleteTrip(id, cancellationToken);
        return new JsonResult(result);
    }

    private async Task<TripDto?> UpdateView(Guid id, CancellationToken cancellationToken)
    {
        var trip = await _trips.FindTrip(id, cancellationToken);

        if (trip == null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Fehler beim Laden der Fahrt.");
            return null;
        }

        var dc = new GermanDateTimeConverter();
        Details = dc.Format(trip.Start, trip.End) + " - " + trip.Target;

        var users = await _identityRepository.GetAll(cancellationToken);

        var contactId = Guid.TryParse(Input.UserId, out var cid) ? cid : User.GetId();

        var items = new List<SelectListItem> { new() };
        items.AddRange(users
            .Where(u => u.Roles!.Any(r => r == Roles.TripManager))
            .Select(u => new SelectListItem(u.Name, u.Id.ToString(), u.Id == contactId)));
        Users = items.ToArray();

        return ModelState.IsValid ? trip : default;
    }
}
