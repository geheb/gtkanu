using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Clubhouse;

[Node("Buchung bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,housemanager")]
public class EditBookingModel : PageModel
{
    private readonly IClubhouseBookings _clubhouse;

    [BindProperty]
    public ClubhouseBookingInput Input { get; set; } = new();

    public bool IsDisabled { get; set; }

    public EditBookingModel(IClubhouseBookings clubhouse)
    {
        _clubhouse = clubhouse;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var booking = await UpdateView(id, cancellationToken);
        if (booking is null) return;
        Input.From(booking);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        if (await UpdateView(id, cancellationToken) is null)
        {
            return Page();
        }

        var error = Input.Validate();
        if (!string.IsNullOrEmpty(error))
        {
            ModelState.AddModelError(string.Empty, error);
            return Page();
        }

        var dto = Input.ToDto(id);

        var status = await _clubhouse.UpdateBooking(dto, cancellationToken);
        if (status != ClubhouseBookingStatus.Success)
        {
            if (status == ClubhouseBookingStatus.Exists)
            {
                ModelState.AddModelError(string.Empty, "Das Vereinsheim ist bereits belegt. Bitte ein anderes Datum wählen.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Fehler beim Anlegen der Buchung.");
            }
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task<ClubhouseBookingDto?> UpdateView(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _clubhouse.FindBooking(id, cancellationToken);

        if (booking is null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Fehler beim Laden der Buchung.");
            return null;
        }

        return ModelState.IsValid ? booking : null;
    }
}
