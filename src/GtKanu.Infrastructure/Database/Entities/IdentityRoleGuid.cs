using Microsoft.AspNetCore.Identity;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class IdentityRoleGuid : IdentityRole<Guid>
{
    public ICollection<IdentityUserRoleGuid>? UserRoles { get; set; }
}
