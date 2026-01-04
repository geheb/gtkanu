namespace GtKanu.WebApp.Pages.Wiki;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

[Node("Artikel bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,wikimanager")]
public class EditArticleModel : PageModel
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentities _identityRepository;

    [BindProperty]
    public WikiInput Input { get; set; } = new();
    public SelectListItem[] Users { get; set; } = Array.Empty<SelectListItem>();
    public bool IsDisabled { get; set; }

    public EditArticleModel(
        IUnitOfWork unitOfWork,
        IIdentities identityRepository)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dto = await UpdateView(id, cancellationToken);
        if (dto is not null)
        {
            Input.FromDto(dto.Value);
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var currentDto = await UpdateView(id, cancellationToken);
        if (currentDto is null) return Page();

        var isSameIdentifier = currentDto.Value.Identifier!.Equals(Input.Identifier!.Trim(), StringComparison.OrdinalIgnoreCase);

        if (!isSameIdentifier &&
            await _unitOfWork.WikiArticles.FindIdentifier(Input.Identifier!, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Die Kennung existiert bereits.");
            return Page();
        }

        var dto = Input.ToDto(id);
        var result = await _unitOfWork.WikiArticles.Update(dto, cancellationToken);

        if (result.IsFailed)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Aktualisieren des Artikels.");
            return Page();
        }

        if (await _unitOfWork.Save(cancellationToken) < 1)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Speichern des Artikels.");
            return Page();
        }

        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.WikiArticles.Delete(id, cancellationToken);
        return new JsonResult(result);
    }

    private async Task<WikiArticleDto?> UpdateView(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _unitOfWork.WikiArticles.Find(id, cancellationToken);
        if (dto == null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Artikel wurde nicht gefunden.");
            return null;
        }

        var users = await _identityRepository.GetAll(cancellationToken);

        var items = new List<SelectListItem> { new() };

        items.AddRange(users
            .Where(u => u.Roles!.Any(r => r == Roles.Member))
            .Select(u => new SelectListItem(u.Name, u.Id.ToString())));

        Users = items.ToArray();

        return ModelState.IsValid ? dto : null;
    }
}
