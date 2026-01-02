using GtKanu.WebApp.Annotations;
using GtKanu.WebApp.Converter;
using GtKanu.WebApp.I18n;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Login;

[AllowAnonymous]
public class ConfirmRegistrationModel : PageModel
{
    private readonly Core.Repositories.IdentityRepository _identityRepository;
    private readonly Core.User.UserService _userService;

    public string ConfirmedEmail { get; set; } = "n.v.";
    public bool IsDisabled { get; set; }

    [BindProperty, Display(Name = "Passwort")]
    [RequiredField, PasswordLengthField]
    public string? Password { get; set; }

    [BindProperty, Display(Name = "Passwort wiederholen")]
    [RequiredField, PasswordLengthField]
    [CompareField(nameof(Password))]
    public string? RepeatPassword { get; set; }

    public ConfirmRegistrationModel(
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

        var result = await _userService.VerifyConfirmRegistration(id, token);
        if (result.IsFailed)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidRegisterConfirmationLink);
            return;
        }

        var user = await _identityRepository.Find(id, cancellationToken);
        if (user is null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidPasswordResetLink);
            return;
        }

        ConfirmedEmail = new EmailConverter().Anonymize(user.Value.Email!);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, string token, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty ||
            string.IsNullOrWhiteSpace(token))
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

        ConfirmedEmail = new EmailConverter().Anonymize(user.Value.Email!);

        var result = await _userService.ConfirmRegistration(id, token);
        if (result.IsFailed)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, LocalizedMessages.InvalidPasswordResetLink);
            return Page();
        }

        result = await _userService.UpdatePassword(id, Password!);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        return RedirectToPage("Index", new { message = 1 });
    }
}
