using GtKanu.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Login;

[Authorize]
public class ExitModel : PageModel
{
    private readonly ILoginService _loginService;

    public ExitModel(ILoginService loginService)
    {
        _loginService = loginService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _loginService.SignOutCurrentUser();
        return Redirect("/");
    }
}
