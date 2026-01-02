namespace GtKanu.WebApp.Pages.Wiki;

using GtKanu.Core.Repositories;
using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

[Node("Artikel anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,wikimanager")]
public class CreateArticleModel : PageModel
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IdentityRepository _identityRepository;

    [BindProperty]
    public WikiInput Input { get; set; } = new();
    public SelectListItem[] Users { get; set; } = Array.Empty<SelectListItem>();

    public CreateArticleModel(
        UnitOfWork unitOfWork, 
        IdentityRepository identityRepository)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await UpdateView(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await UpdateView(cancellationToken)) return Page();

        if (await _unitOfWork.WikiArticles.FindIdentifier(Input.Identifier!, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Die Kennung existiert bereits.");
            return Page();
        }

        var dto = Input.ToDto();

        await _unitOfWork.WikiArticles.Create(dto, cancellationToken);
        var result = await _unitOfWork.Save(cancellationToken) > 0;
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Anlegen des Artikels.");
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task<bool> UpdateView(CancellationToken cancellationToken)
    {
        var users = await _identityRepository.GetAll(cancellationToken);

        var items = new List<SelectListItem> { new() };

        items.AddRange(users
            .Where(u => u.Roles!.Any(r => r == Roles.Member))
            .Select(u => new SelectListItem(u.Name, u.Id.ToString())));

        Users = items.ToArray();

        return ModelState.IsValid;
    }
}
