using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.Clubhouse;

[Node("Buchung anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,housemanager")]
public class CreateBookingModel : PageModel
{
    private readonly Core.Repositories.Clubhouse _clubhouse;

    [BindProperty]
    public ClubhouseBookingInput Input { get; set; } = new();

    public CreateBookingModel(Core.Repositories.Clubhouse clubhouse)
    {
        _clubhouse = clubhouse;
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
