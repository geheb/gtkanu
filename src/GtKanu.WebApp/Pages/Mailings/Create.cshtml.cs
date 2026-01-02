using GtKanu.Core.Email;
using GtKanu.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace GtKanu.WebApp.Pages.Mailings;

[Node("Mailing anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,mailingmanager")]
public class CreateModel : PageModel
{
    private readonly EmailValidatorService _emailValidator;
    private readonly UnitOfWork _unitOfWork;

    [BindProperty]
    public MailingInput Input { get; set; } = new();


    public CreateModel(
        IOptions<SmtpConnectionOptions> smtpOptions,
        IOptions<AppSettings> appOptions,
        EmailValidatorService emailValidator,
        UnitOfWork unitOfWork)
    {
        Input.ReplyAddress = appOptions.Value.MailingReplyTo ?? smtpOptions.Value.SenderEmail;
        _emailValidator = emailValidator;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var validation = await Input.Validate(_emailValidator, cancellationToken);
        if (validation.Length > 0)
        {
            Array.ForEach(validation, v => ModelState.AddModelError(string.Empty, v));
            return Page();
        }

        await _unitOfWork.Mailings.Create(Input.ToDto(), cancellationToken);
        if (await _unitOfWork.Save(cancellationToken) < 1)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Anlegen des Mailings.");
            return Page();
        }

        return RedirectToPage("Index");
    }
}
