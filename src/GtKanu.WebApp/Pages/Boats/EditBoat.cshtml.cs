using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.Boats;

[Node("Boot bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,boatmanager")]
public class EditBoatModel : PageModel
{
    private readonly Core.Repositories.Boats _boats;

    [BindProperty]
    public BoatInput Input { get; set; } = new();

    public bool IsDisabled { get; set; }

    public EditBoatModel(Core.Repositories.Boats boats)
    {
        _boats = boats;
    }

    public async Task OnGet(Guid id, CancellationToken cancellationToken)
    {
        var boat = await _boats.FindBoat(id, cancellationToken);
        if (boat is null)
        {
            ModelState.AddModelError(string.Empty, "Das Boot wurde nicht gefunden.");
            IsDisabled = true;
            return;
        }

        Input.From(boat);
    }

    public async Task<IActionResult> OnPost(Guid id, CancellationToken cancellationToken)
    {
        var hasBoat = await _boats.FindBoat(id, cancellationToken) is not null;
        if (!hasBoat)
        {
            ModelState.AddModelError(string.Empty, "Das Boot wurde nicht gefunden.");
            IsDisabled = true;
            return Page();
        }

        var dto = Input.ToDto();
        dto.Id = id;

        var status = await _boats.Update(dto, cancellationToken);

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