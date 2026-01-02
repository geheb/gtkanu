using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Login;

[Authorize]
public class ExitModel : PageModel
{
    private readonly Core.User.LoginService _loginService;

    public ExitModel(Core.User.LoginService loginService)
    {
        _loginService = loginService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _loginService.SignOutCurrentUser();
        return Redirect("/");
    }
}
