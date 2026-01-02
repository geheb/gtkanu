using GtKanu.Core.Email;
using GtKanu.Core.Extensions;
using GtKanu.Core.Migrations;
using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Scriban.Parsing;

namespace GtKanu.WebApp.Pages.Users;

[Node("Benutzer anlegen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,usermanager")]
public class CreateUserModel : PageModel
{
    private readonly UserService _userService;
    private readonly EmailValidatorService _emailValidatorService;
    private readonly Core.Repositories.IdentityRepository _identityRepository;

    [BindProperty]
    public CreateUserInput Input { get; set; } = new();

    public CreateUserModel(
        UserService userService,
        EmailValidatorService emailValidatorService,
        Core.Repositories.IdentityRepository identityRepository)
    {
        _userService = userService;
        _emailValidatorService = emailValidatorService;
        _identityRepository = identityRepository;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();

        if (Input.Roles.All(r => r == false))
        {
            ModelState.AddModelError(string.Empty, "Keine Rolle ausgewählt.");
            return Page();
        }

        if (!await _emailValidatorService.Validate(Input.Email!, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Die E-Mail-Adresse ist ungültig.");
            return Page();
        }

        var dto = Input.ToDto();

        var result = await _identityRepository.Create(dto, cancellationToken);

        if (result.IsFailed)
        {
            result.Errors.ToModelState(ModelState);
            return Page();
        }

        var callbackUrl = Url.PageLink("/Login/ConfirmRegistration", values: new { id = Guid.Empty, token = string.Empty });

        result = await _userService.NotifyConfirmRegistration(dto.Email!, callbackUrl!, cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ToModelState(ModelState);
            return Page();
        }

        return RedirectToPage("Index");
    }
}
