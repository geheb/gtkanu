namespace GtKanu.Ui.Pages.Trips;

using GtKanu.Core.Repositories;
using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

[Node("Fahrt anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,tripmanager")]
public class CreateTripModel : PageModel
{
    private readonly IdentityRepository _identityRepository;
    private readonly Trips _trips;

    [BindProperty]
    public TripInput Input { get; set; } = new();

    public SelectListItem[] Users { get; set; } = Array.Empty<SelectListItem>();

    public CreateTripModel(
        IdentityRepository identityRepository, 
        Trips trips)
    {
        _identityRepository = identityRepository;
        _trips = trips;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await UpdateView(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await UpdateView(cancellationToken)) return Page();

        var error = Input.Validate();
        if (!string.IsNullOrEmpty(error))
        {
            ModelState.AddModelError(string.Empty, error);
            return Page();
        }

        var dto = Input.ToDto();

        var result = await _trips.CreateTrip(dto, cancellationToken);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Anlegen der Fahrt.");
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task<bool> UpdateView(CancellationToken cancellationToken)
    {
        var users = await _identityRepository.GetAll(cancellationToken);

        var contactId = Guid.TryParse(Input.UserId, out var id) ? id : User.GetId();

        var items = new List<SelectListItem> { new() };
        items.AddRange(users
            .Where(u => u.Roles!.Any(r => r == Roles.TripManager))
            .Select(u => new SelectListItem(u.Name, u.Id.ToString(), u.Id == contactId)));
        Users = items.ToArray();

        return ModelState.IsValid;
    }
}
