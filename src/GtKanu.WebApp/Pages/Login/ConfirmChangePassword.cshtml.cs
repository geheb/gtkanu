using GtKanu.Core.Extensions;
using GtKanu.WebApp.Annotations;
using GtKanu.WebApp.Converter;
using GtKanu.WebApp.I18n;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Login;

[AllowAnonymous]
public class ConfirmChangePasswordModel : PageModel
{
    private readonly Core.Repositories.IdentityRepository _identityRepository;
    private readonly Core.User.UserService _userService;

    [BindProperty]
    public string? UserName { get; set; } // just for bots

    [BindProperty, Display(Name = "Passwort")]
    [RequiredField, PasswordLengthField]
    public string? Password { get; set; }

    [BindProperty, Display(Name = "Passwort wiederholen")]
    [RequiredField, PasswordLengthField]
    [CompareField(nameof(Password))]
    public string? RepeatPassword { get; set; }

    public bool IsDisabled { get; set; }
    public string? ChangePasswordEmail { get; set; } = "n.v.";

    public ConfirmChangePasswordModel(
        Core.Repositories.IdentityRepository identityRepository,
        Core.User.UserService userService)
    {
        _identityRepository = identityRepository;
        _userService = userService;
    }

    public async Task OnGetAsync(Guid id, string token, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty || 
            string.IsNullOrWhiteSpace(token))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidRequest);
            return;
        }

        var result = await _userService.VerifyChangePassword(id, token);
        if (result.IsFailed)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidPasswordResetLink);
            return;
        }

        var user = await _identityRepository.Find(id, cancellationToken);
        if (user is null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidPasswordResetLink);
            return;
        }

        ChangePasswordEmail = new EmailConverter().Anonymize(user.Value.Email!);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, string token, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty || 
            string.IsNullOrWhiteSpace(token) || 
            !string.IsNullOrEmpty(UserName))
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidRequest);
            return Page();
        }

        if (!ModelState.IsValid) return Page();

        var user = await _identityRepository.Find(id, cancellationToken);
        if (user is null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidPasswordResetLink);
            return Page();
        }

        ChangePasswordEmail = new EmailConverter().Anonymize(user.Value.Email!);

        var result = await _userService.ConfirmChangePassword(id, token, Password!);
        if (result.IsFailed)
        {
            result.Errors.ToModelState(ModelState);
            return Page();
        }

        return RedirectToPage("Index", new { message = 1 });
    }
}
