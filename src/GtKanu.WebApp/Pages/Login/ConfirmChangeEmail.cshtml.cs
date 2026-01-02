using GtKanu.Ui.Converter;
using GtKanu.Ui.I18n;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.Login;

[AllowAnonymous]
public class ConfirmChangeEmailModel : PageModel
{
    private readonly Core.User.UserService _userService;

    public string? ConfirmedEmail { get; set; }

    public ConfirmChangeEmailModel(Core.User.UserService userService)
    {
        _userService = userService;
    }

    public async Task OnGetAsync(Guid id, string token, string email)
    {
        if (id == Guid.Empty || 
            string.IsNullOrWhiteSpace(token) || 
            string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidRequest);
            return;
        }

        var result = await _userService.ConfirmChangeEmail(id, token, email);
        if (result.IsFailed)
        {
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidNewEmailConfirmationLink);
            return;
        }

        ConfirmedEmail = new EmailConverter().Anonymize(email);
    }
}
