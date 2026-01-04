using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Application.Services;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace GtKanu.WebApp.Pages.Mailings;

[Node("Mailing anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,mailingmanager")]
public class CreateModel : PageModel
{
    private readonly IEmailValidatorService _emailValidator;
    private readonly IUnitOfWork _unitOfWork;

    [BindProperty]
    public MailingInput Input { get; set; } = new();


    public CreateModel(
        IOptions<SmtpConnectionOptions> smtpOptions,
        IOptions<AppSettings> appOptions,
        IEmailValidatorService emailValidator,
        IUnitOfWork unitOfWork)
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

        _unitOfWork.Mailings.Create(Input.ToDto());
        if (await _unitOfWork.Save(cancellationToken) < 1)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Anlegen des Mailings.");
            return Page();
        }

        return RedirectToPage("Index");
    }
}
