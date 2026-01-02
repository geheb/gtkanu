using GtKanu.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Users;

[Node("Benutzer bearbeiten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,usermanager", Policy = Policies.TwoFactorAuth)]
public class EditUserModel : PageModel
{
    private readonly Core.User.UserService _userService;
    private readonly Core.Repositories.IdentityRepository _identityRepository;

    [BindProperty]
    public EditUserInput Input { get; set; } = new();

    public bool IsDisabled { get; set; }
    public string? Name { get; set; }

    public EditUserModel(
        Core.User.UserService userService,
        Core.Repositories.IdentityRepository identityRepository)
    {
        _userService = userService;
        _identityRepository = identityRepository;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _identityRepository.Find(id, cancellationToken);
        if (user is null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Benutzer wurde nicht gefunden.");
            return;
        }

        Input.From(user.Value);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _identityRepository.Find(id, cancellationToken);
        if (user is null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Benutzer wurde nicht gefunden.");
            return Page();
        }

        if (Input.Roles.All(r => r == false))
        {
            ModelState.AddModelError(string.Empty, "Keine Rolle ausgewählt.");
            return Page();
        }

        if (!ModelState.IsValid) return Page();

        var dto = Input.ToDto(user.Value.Id);

        var result = await _identityRepository.Update(dto, cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ToModelState(ModelState);
            return Page();
        }

        if (!string.IsNullOrEmpty(Input.Password))
        {
            result = await _userService.UpdatePassword(user.Value.Id, Input.Password);
            if (result.IsFailed)
            {
                result.Errors.ToModelState(ModelState);
                return Page();
            }
        }

        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostConfirmEmailAsync(Guid id, CancellationToken cancellationToken)
    {
        var callbackUrl = Url.PageLink("/Login/ConfirmRegistration", values: new { id = Guid.Empty, token = string.Empty });
        var result = await _userService.ReNotifyConfirmRegistration(id, callbackUrl!, cancellationToken);
        return new JsonResult(result.IsSuccess);
    }

    public async Task<IActionResult> OnPostRemoveUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _identityRepository.Remove(id, cancellationToken);
        return new JsonResult(result.IsSuccess);
    }

    public async Task<IActionResult> OnPostResetTwoFactorAsync(Guid id)
    {
        var result = await _userService.ResetTwoFactor(id);
        return new JsonResult(result.IsSuccess);
    }
}
