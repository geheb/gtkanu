using GtKanu.Application.Repositories;
using GtKanu.Application.Services;
using GtKanu.Infrastructure.AspNetCore.Annotations;
using GtKanu.Infrastructure.Extensions;
using GtKanu.WebApp.Converter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Login;

[AllowAnonymous]
public class ConfirmRegistrationModel : PageModel
{
    private readonly IIdentities _identityRepository;
    private readonly IUserService _userService;

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
        IIdentities identityRepository,
        IUserService userService)
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
            ModelState.AddModelError(string.Empty, "Die Anfrage ist ungültig.");
            return;
        }

        var result = await _userService.VerifyConfirmRegistration(id, token);
        if (result.IsFailed)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Der Link zum Bestätigen der Registrierung ist ungültig oder abgelaufen. Der Vorgang muss wiederholt werden.");
            return;
        }

        var user = await _identityRepository.Find(id, cancellationToken);
        if (user is null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Der Link zum Bestätigen der Registrierung ist ungültig oder abgelaufen. Der Vorgang muss wiederholt werden.");
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
            ModelState.AddModelError(string.Empty, "Die Anfrage ist ungültig.");
            return Page();
        }

        if (!ModelState.IsValid) return Page();

        var user = await _identityRepository.Find(id, cancellationToken);
        if (user is null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Der Link zum Bestätigen der Registrierung ist ungültig oder abgelaufen. Der Vorgang muss wiederholt werden.");
            return Page();
        }

        ConfirmedEmail = new EmailConverter().Anonymize(user.Value.Email!);

        var result = await _userService.ConfirmRegistration(id, token);
        if (result.IsFailed)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Der Link zum Bestätigen der Registrierung ist ungültig oder abgelaufen. Der Vorgang muss wiederholt werden.");
            return Page();
        }

        result = await _userService.UpdatePassword(id, Password!);
        if (result.IsFailed)
        {
            result.Errors.ToModelState(ModelState);
            return Page();
        }

        return RedirectToPage("Index", new { message = 1 });
    }
}
