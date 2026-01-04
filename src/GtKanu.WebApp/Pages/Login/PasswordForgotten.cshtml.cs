using GtKanu.Application.Services;
using GtKanu.Infrastructure.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Login;

[AllowAnonymous]
public class PasswordForgottenModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IEmailValidatorService _emailValidator;

    [BindProperty]
    public string? UserName { get; set; } // just for Bots

    [BindProperty, Display(Name = "E-Mail-Adresse")]
    [RequiredField, EmailLengthField, EmailField]
    public string? Email { get; set; }

    [BindProperty]
    public bool IsDisabled { get; set; }

    public PasswordForgottenModel(
        IEmailValidatorService emailValidator,
        IUserService userService)
    {
        _emailValidator = emailValidator;
        _userService = userService;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(UserName))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Die Anfrage ist ungültig.");
            return Page();
        }

        if (!ModelState.IsValid) return Page();

        if (!await _emailValidator.Validate(Email!, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Die E-Mail-Adresse ist ungültig.");
            return Page();
        }

        var callbackUrl = Url.PageLink("/Login/ConfirmChangePassword", values: new { id = Guid.Empty, token = string.Empty });

        await _userService.NotifyChangePassword(Email!, callbackUrl!, cancellationToken);

        return RedirectToPage("Index", new { message = 2 });
    }
}
