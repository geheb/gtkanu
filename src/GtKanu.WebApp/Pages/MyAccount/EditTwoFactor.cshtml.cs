using GtKanu.Core.User;
using GtKanu.WebApp.Annotations;
using GtKanu.WebApp.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using QRCoder;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.MyAccount;

[Node("Zwei-Faktor-Authentifizierung bearbeiten", FromPage = typeof(IndexModel))]
[Authorize]
public class EditTwoFactorModel : PageModel
{
    private readonly string _appName;
    private readonly Core.User.UserService _userService;
    private readonly NodeGeneratorService _nodeGeneratorService;

    [BindProperty, Display(Name = "6-stelliger Code aus der Authenticator-App")]
    [RequiredField, TextLengthField(6, MinimumLength = 6)]
    public string? Code { get; set; }

    [Display(Name = "Geheimer Schlüssel")]
    public string? SecretKey { get; set; }
    public string? AuthUri { get; set; }
    public string? AuthQrCodeEncoded { get; set; }
    public bool IsTwoFactorEnabled { get; set; }

    public bool IsDisabled { get; set; }

    public EditTwoFactorModel(
        Core.User.UserService userService,
        NodeGeneratorService nodeGeneratorService,
        IOptions<AppSettings> appOptions)
    {
        _appName = appOptions.Value.HeaderTitle;
        _userService = userService;
        _nodeGeneratorService = nodeGeneratorService;
    }

    public Task OnGetAsync(CancellationToken cancellationToken) => UpdateView(cancellationToken);

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await UpdateView(cancellationToken))
        {
            return Page();
        }

        var enable = !IsTwoFactorEnabled;

        var result = await _userService.EnableTwoFactor(User.GetId(), enable, Code!);
        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            return Page();
        }

        if (!enable)
        {
            Response.Cookies.Delete(CookieNames.TwoFactorTrustToken);
        }

        return RedirectToPage(_nodeGeneratorService.GetNode<IndexModel>().Page, new { message = enable ? 3 : 4 });
    }

    private async Task<bool> UpdateView(CancellationToken cancellationToken)
    {
        var result = await _userService.CreateTwoFactor(User.GetId(), _appName);

        if (result.IsFailed)
        {
            result.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e.Message));
            IsDisabled = true;
            return false;
        }

        IsTwoFactorEnabled = result.Value.IsEnabled;
        SecretKey = result.Value.SecretKey;
        AuthUri = result.Value.AuthUri;
        AuthQrCodeEncoded = GenerateQrCodeEncoded(result.Value.AuthUri);

        return ModelState.IsValid;
    }

    private static string GenerateQrCodeEncoded(string data)
    {
        using var generator = new QRCodeGenerator();
        using var code = generator.CreateQrCode(data, QRCodeGenerator.ECCLevel.H);
        using var image = new PngByteQRCode(code);
        var content = image.GetGraphic(5);
        return "data:image/png;base64," + Convert.ToBase64String(content);
    }
}
