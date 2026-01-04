using GtKanu.Application.Services;
using GtKanu.WebApp.Converter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Login;

[AllowAnonymous]
public class ConfirmChangeEmailModel : PageModel
{
    private readonly IUserService _userService;

    public string? ConfirmedEmail { get; set; }

    public ConfirmChangeEmailModel(IUserService userService)
    {
        _userService = userService;
    }

    public async Task OnGetAsync(Guid id, string token, string email)
    {
        if (id == Guid.Empty || 
            string.IsNullOrWhiteSpace(token) || 
            string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(string.Empty, "Anfrage ist ungültig.");
            return;
        }

        var result = await _userService.ConfirmChangeEmail(id, token, email);
        if (result.IsFailed)
        {
            ModelState.AddModelError(string.Empty, "Der Link zum Bestätigen der neuen E-Mail-Adresse ist ungültig oder abgelaufen. Der Vorgang muss wiederholt werden.");
            return;
        }

        ConfirmedEmail = new EmailConverter().Anonymize(email);
    }
}
