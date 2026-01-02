using GtKanu.Core.Email;
using GtKanu.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Mailings;

[Node("Mailing bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,mailingmanager")]
public class EditModel : PageModel
{
    private readonly UnitOfWork _unitOfWork;
    private readonly EmailService _emailService;

    [BindProperty]
    public MailingInput Input { get; set; } = new();

    public bool IsDisabled { get; set; }

    public EditModel(
        UnitOfWork unitOfWork,
        EmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _unitOfWork.Mailings.Find(id, cancellationToken);
        if (dto is null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Das Mailing wurde nicht gefunden.");
            return;
        }

        if (dto.Value.IsClosed)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Das Mailing ist bereits abgeschlossen.");
        }

        Input.From(dto.Value);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, int action, CancellationToken cancellationToken)
    {
        var mailingDto = await _unitOfWork.Mailings.Find(id, cancellationToken);
        if (mailingDto is null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Das Mailing wurde nicht gefunden.");
            return Page();
        }
        
        if (action == 0)
        {
            var dto = Input.ToDto(id);

            var result = await _unitOfWork.Mailings.Update(dto, cancellationToken);
            if (result.IsFailed)
            {
                ModelState.AddModelError(string.Empty, "Fehler beim Aktualisieren des Mailings.");
                return Page();
            }

            if (await _unitOfWork.Save(cancellationToken) < 1)
            {
                ModelState.AddModelError(string.Empty, "Fehler beim Speichern des Mailings.");
                return Page();
            }
        }
        else
        {
            var result = await _emailService.EnqueMailing(id, cancellationToken);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Fehler beim Anlegen des Mailingversands.");
                return Page();
            }
        }

        return RedirectToPage("Index");
    }
}
