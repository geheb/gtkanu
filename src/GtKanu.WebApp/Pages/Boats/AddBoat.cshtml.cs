using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Boats;

[Node("Boot anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,boatmanager")]
public class AddBoatModel : PageModel
{
    private readonly Core.Repositories.Boats _boats;

    [BindProperty]
    public BoatInput Input { get; set; } = new();

    public AddBoatModel(Core.Repositories.Boats boats)
    {
        _boats = boats;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var dto = Input.ToDto();
        var status = await _boats.Create(dto, cancellationToken);

        if (status != BoatStatus.Success)
        {
            if (status == BoatStatus.Exists)
            {
                ModelState.AddModelError(string.Empty, "Das Boot mit der Nummer existiert bereits. Bitte eine andere Nummer wählen.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Fehler beim Anlegen des Bootes.");
            }
            return Page();
        }

        return RedirectToPage("Index");
    }
}
