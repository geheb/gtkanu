using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Clubhouse;

[Node("Buchung anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,housemanager")]
public class CreateBookingModel : PageModel
{
    private readonly IClubhouse _clubhouse;

    [BindProperty]
    public ClubhouseBookingInput Input { get; set; } = new();

    public CreateBookingModel(IClubhouse clubhouse)
    {
        _clubhouse = clubhouse;
        var dc = new GermanDateTimeConverter();
        Input.Start = dc.ToIso(dc.ToLocal(DateTimeOffset.UtcNow));
        Input.End = dc.ToIso(dc.ToLocal(DateTimeOffset.UtcNow.AddDays(1)));
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var error = Input.Validate();
        if (!string.IsNullOrEmpty(error))
        {
            ModelState.AddModelError(string.Empty, error);
            return Page();
        }

        var dto = Input.ToDto();

        var status = await _clubhouse.CreateBooking(dto, cancellationToken);

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
}
