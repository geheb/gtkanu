using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GtKanu.Ui.Pages.Tryouts;

[Node("Trainingsserie anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,tripmanager")]
public class CreateRecurringModel : PageModel
{
    private readonly Core.Repositories.IdentityRepository _identityRepository;
    private readonly Core.Repositories.Tryouts _tryouts;

    [BindProperty]
    public RecurringInput Input { get; set; } = new();

    public SelectListItem[] Users { get; private set; } = Array.Empty<SelectListItem>();

    public CreateRecurringModel(
        Core.Repositories.IdentityRepository identityRepository, 
        Core.Repositories.Tryouts tryouts)
    {
        _identityRepository = identityRepository;
        _tryouts = tryouts;
    }

    public Task OnGetAsync(CancellationToken cancellationToken) => UpdateView(cancellationToken);

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await UpdateView(cancellationToken)) return Page();

        var error = Input.Validate();
        if (!string.IsNullOrEmpty(error))
        {
            ModelState.AddModelError(string.Empty, error);
            return Page();
        }
        var dtos = Input.ToDtos();

        var result = await _tryouts.CreateTryout(dtos, cancellationToken);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Anlegen der Terminserie.");
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
