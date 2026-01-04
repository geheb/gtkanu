using GtKanu.Application.Services;
using GtKanu.Infrastructure.AspNetCore.Annotations;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.Extensions;
using GtKanu.Infrastructure.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.MyAccount;

[Node("Passwort ändern", FromPage = typeof(IndexModel))]
[Authorize]
public class ChangePasswordModel : PageModel
{
    private readonly IUserService _userService;

    [BindProperty, Display(Name = "Aktuelles Passwort")]
    [RequiredField, PasswordLengthField(MinimumLength = 8)] // old passwords has 8
    public string? CurrentPassword { get; set; }

    [BindProperty, Display(Name = "Neues Passwort")]
    [RequiredField, PasswordLengthField]
    public string? NewPassword { get; set; }

    [BindProperty, Display(Name = "Neues Passwort bestätigen")]
    [RequiredField, PasswordLengthField]
    [CompareField(nameof(NewPassword))]
    public string? ConfirmNewPassword { get; set; }

    public bool IsDisabled { get; set; }

    public ChangePasswordModel(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _userService.ChangePassword(User.GetId(), CurrentPassword!, NewPassword!);
        if (result.IsFailed)
        {
            result.Errors.ToModelState(ModelState);
            return Page();
        }

        return RedirectToPage("Index", new { message = 1 });
    }
}
