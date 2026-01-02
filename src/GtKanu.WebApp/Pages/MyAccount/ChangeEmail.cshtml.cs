using GtKanu.Core.Extensions;
using GtKanu.Core.User;
using GtKanu.WebApp.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GtKanu.WebApp.Pages.MyAccount;

[Node("E-Mail-Adresse ändern", FromPage = typeof(IndexModel))]
[Authorize]
public class ChangeEmailModel : PageModel
{
    private readonly Core.Email.EmailValidatorService _emailValidatorService;
    private readonly Core.User.UserService _userService;

    [Display(Name = "Aktuelle E-Mail-Adresse")]
    public string? CurrentEmail { get; private set; }

    [BindProperty, Display(Name = "Neue E-Mail-Adresse")]
    [RequiredField, EmailLengthField, EmailField]
    public string? NewEmail { get; set; }

    [BindProperty, Display(Name = "Aktuelles Passwort")]
    [RequiredField, PasswordLengthField(MinimumLength = 8)] // old passwords has 8
    public string? CurrentPassword { get; set; }

    public bool IsDisabled { get; set; }

    public ChangeEmailModel(
        Core.Email.EmailValidatorService emailValidatorService,
        Core.User.UserService userService)
    {
        _emailValidatorService = emailValidatorService;
        _userService = userService;
    }

    public void OnGet()
    {
        CurrentEmail = User.FindFirstValue(ClaimTypes.Email);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        CurrentEmail = User.FindFirstValue(ClaimTypes.Email);

        if (!ModelState.IsValid) return Page();

        var result = await _userService.VerifyPassword(User.GetId(), CurrentPassword!);
        if (result.IsFailed)
        {
            result.Errors.ToModelState(ModelState);
            return Page();
        }

        if (!await _emailValidatorService.Validate(NewEmail!, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Die neue E-Mail-Adresse ist ungültig.");
            return Page();
        }

        var callbackUrl = Url.PageLink("/Login/ConfirmChangeEmail", values: new { id = Guid.Empty, token = string.Empty, email = string.Empty });

        result = await _userService.NotifyChangeEmail(User.GetId(), NewEmail!, callbackUrl!, cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ToModelState(ModelState);
            return Page();
        }

        return RedirectToPage("Index", new { message = 2 });
    }
}
