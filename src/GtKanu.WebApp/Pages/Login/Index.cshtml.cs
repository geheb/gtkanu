using GtKanu.WebApp.Annotations;
using GtKanu.WebApp.I18n;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Login;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly Core.User.LoginService _loginService;

    [BindProperty]
    public string? UserName { get; set; } // just for bots

    [BindProperty, Display(Name = "E-Mail-Adresse")]
    [RequiredField, EmailLengthField, EmailField]
    public string? Email { get; set; }

    [BindProperty, Display(Name = "Passwort")]
    [RequiredField, PasswordLengthField(MinimumLength = 8)] // old passwords has 8
    public string? Password { get; set; }

    public string? Message { get; set; }

    public IndexModel(
        Core.User.LoginService loginService)
    {
        _loginService = loginService;
    }

    public void OnGet(int message = 0)
    {
        if (message == 1)
        {
            Message = "Das Passwort wurde geändert. Melde dich jetzt mit dem neuen Passwort an.";
        }
        else if (message == 2)
        {
            Message = "Falls deine E-Mail-Adresse existiert, wird eine E-Mail an diese versendet. Mit Hilfe der E-Mail kannst du das Passwort ändern.";
        }
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(UserName))
        {
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidRequest);
            return Page();
        }

        if (!ModelState.IsValid) return Page();

        var result = await _loginService.SignIn(Email!, Password!, cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        if (result.Value)
        {
            return RedirectToPage("ConfirmTwoFactor", new { returnUrl });
        }

        return !string.IsNullOrEmpty(returnUrl) ? LocalRedirect(returnUrl) : Redirect("/");
    }
}
