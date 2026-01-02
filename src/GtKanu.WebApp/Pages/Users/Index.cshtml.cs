using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.Users;

[Node("Benutzer", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,usermanager", Policy = Policies.TwoFactorAuth)]
public class IndexModel : PageModel
{
    private readonly Core.Repositories.IdentityRepository _identityRepository;

    public IdentityDto[] Users { get; private set; } = [];
    public int UsersConfirmed { get; set; }
    public int UsersNotConfirmed { get; set; }
    public int UsersLocked { get; set; }

    public IndexModel(Core.Repositories.IdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task OnGetAsync(int filter, CancellationToken cancellationToken)
    {
        var users = await _identityRepository.GetAll(cancellationToken);
        if (filter == 1)
        {
            Users = [.. users.Where(u => u.Roles!.Contains(Roles.Interested))];
        }
        else if (filter == 2)
        {
            Users = [.. users.Where(u => Roles.IsMemberWithRole(u.Roles!))];
        }
        else if (filter == 3)
        {
            Users = [.. users.Where(u => u.Mailings?.Any(m => m == UserMailings.YoungPeople) ?? false)];
        }
        else
        {
            Users = users;
        }

        UsersConfirmed = Users.Count(u => u.IsEmailConfirmed);
        UsersNotConfirmed = Users.Count(u => !u.IsEmailConfirmed);
        UsersLocked = Users.Count(u => u.IsLocked);
    }
}
