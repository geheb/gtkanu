using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GtKanu.Ui.Pages.Boats;

[Node("Boot mieten", FromPage = typeof(RentalsModel))]
[Authorize(Roles = "administrator,boatmanager")]
public class CreateRentalModel : PageModel
{
    private readonly Core.Repositories.Boats _boats;
    private readonly Core.Repositories.IdentityRepository _identityRepository;

    [BindProperty]
    public RentalInput Input { get; set; } = new();

    public SelectListItem[] Users { get; private set; } = Array.Empty<SelectListItem>();

    public bool IsDisabled { get; set; }
    public string? BoatDetails { get; set; }
    public bool IsLongterm { get; set; }

    public CreateRentalModel(
        Core.Repositories.Boats boats, 
        Core.Repositories.IdentityRepository identityRepository)
    {
        _boats = boats;
        _identityRepository = identityRepository;
    }

    public Task OnGetAsync(Guid id, CancellationToken cancellationToken) => UpdateView(id, cancellationToken);

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var boat = await UpdateView(id, cancellationToken);
        if (boat is null)
        {
            return Page();
        }

        var error = Input.Validate(boat.MaxRentalDays);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return Page();
        }

        var dto = Input.ToDto(id, User.GetId());
        var status = await _boats.CreateRental(dto, cancellationToken);

        if (status != BoatRentalStatus.Success)
        {
            if (status == BoatRentalStatus.AlreadyBooked)
            {
                ModelState.AddModelError(string.Empty, "Das Datum für die Miete ist bereits vergeben.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Fehler beim Anlegen der Miete.");
            }
            return Page();
        }

        return RedirectToPage("Rentals", new { id });
    }

    private async Task<BoatDto?> UpdateView(Guid boatId, CancellationToken cancellationToken)
    {
        var boat = await _boats.FindBoat(boatId, cancellationToken);
        if (boat is null)
        {
            ModelState.AddModelError(string.Empty, "Das Boot wurde nicht gefunden.");
            IsDisabled = true;
            return null;
        }

        IsLongterm = boat.MaxRentalDays == 0;
        BoatDetails = boat.FullDetails;

        if (boat.IsLocked)
        {
            ModelState.AddModelError(string.Empty, "Das Boot ist gesperrt. Das Mieten ist somit nicht möglich.");
            IsDisabled = true;
            return null;
        }

        if (Input.Start is null)
        {
            var lastBooking = await _boats.GetLastRental(boatId, cancellationToken);
            var dc = new GermanDateTimeConverter();
            var now = dc.ToLocal(DateTimeOffset.UtcNow);

            var start = lastBooking?.End is null || lastBooking.End < now
                ? DateOnly.FromDateTime(now.Date)
                : DateOnly.FromDateTime(lastBooking.End.Date.AddDays(1));

            Input.Start = dc.ToIso(start);
            if (IsLongterm)
            {
                Input.End = dc.ToIso(start.AddYears(10));
            }
        }

        var users = await _identityRepository.GetAll(cancellationToken);
        var contactId = Guid.TryParse(Input.UserId, out var id) ? id : User.GetId();
        
        var items = new List<SelectListItem> { new() };
        items.AddRange(users
            .Where(u => u.IsEmailConfirmed && u.Roles!.Any(r => r == Roles.Member))
            .Select(u => new SelectListItem(u.Name, u.Id.ToString(), u.Id == contactId)));
        
        Users = items.ToArray();

        return ModelState.IsValid ? boat : null;
    }
}
