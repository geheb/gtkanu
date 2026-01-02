using GtKanu.Ui.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.Ui.Pages.Login;

[AllowAnonymous]
public sealed class ConfirmTwoFactorModel : PageModel
{
    private readonly Core.User.LoginService _loginService;

    [BindProperty, Display(Name = "6-stelliger Code aus der Authenticator-App")]
    [RequiredField, TextLengthField(6, MinimumLength = 6)]
    public string? Code { get; set; }

    [BindProperty, Display(Name = "Diesen Browser vertrauen")]
    public bool IsTrustBrowser { get; set; }

    public string? ReturnUrl { get; set; }

    public bool IsDisabled { get; set; }

    public ConfirmTwoFactorModel(
        Core.User.LoginService loginService)
    {
        _loginService = loginService;
    }

    public void OnGet(string? returnUrl) => ReturnUrl = returnUrl;

    public async Task<IActionResult> OnPostAsync(string? returnUrl, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();

        var result = await _loginService.SignInTwoFactor(Code!, IsTrustBrowser, cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        return !string.IsNullOrEmpty(returnUrl) ? LocalRedirect(returnUrl) : Redirect("/");
    }
}
