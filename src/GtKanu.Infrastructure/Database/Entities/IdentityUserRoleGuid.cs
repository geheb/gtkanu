using Microsoft.AspNetCore.Identity;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class IdentityUserRoleGuid : IdentityUserRole<Guid>
{
    public IdentityUserGuid? User { get; set; }
    public IdentityRoleGuid? Role { get; set; }
}
