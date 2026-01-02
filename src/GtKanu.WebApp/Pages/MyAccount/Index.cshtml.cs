using GtKanu.Core.Extensions;
using GtKanu.Core.User;
using GtKanu.WebApp.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.MyAccount;

[Node("Mein Konto", FromPage = typeof(Pages.IndexModel))]
[Authorize]
public class IndexModel : PageModel
{
    private readonly Core.Repositories.IdentityRepository _identityRepository;

    [BindProperty, Display(Name = "E-Mail-Adresse")]
    public string? Email { get; set; }

    [BindProperty, Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    [BindProperty, Display(Name = "Telefonnummer")]
    [PhoneField]
    public string? PhoneNumber { get; set; }

    public bool IsDisabled { get; set; }
    public string? Info { get; set; }

    public IndexModel(Core.Repositories.IdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task OnGetAsync(int? message, CancellationToken cancellationToken)
    {
        Info = message switch
        {
            1 => "Das Passwort wurde geändert.",
            2 => "Eine E-Mail wird an die neue E-Mail-Adresse versendet und muss bestätigt werden - erst dann ist die Änderung vollständig.",
            3 => "2FA wurde aktiviert. Bitte erneut anmelden!",
            4 => "2FA wurde deaktiviert.",
            _ => default
        };

        var user = await _identityRepository.Find(User.GetId(), cancellationToken);
        if (user is null)
        {
            IsDisabled = true;
            return;
        }

        Name = user.Value.Name;
        Email = user.Value.Email;
        PhoneNumber = user.Value.PhoneNumber;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();

        var dto = new IdentityDto
        {
            Id = User.GetId(),
            Name = Name,
            PhoneNumber = PhoneNumber,
        };

        var result = await _identityRepository.Update(dto, cancellationToken);
        if (result.IsFailed)
        {
            result.Errors.ToModelState(ModelState);
            return Page();
        }

        Info = "Änderungen wurden gespeichert.";

        return Page();
    }
}
