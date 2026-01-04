using System.Security.Claims;

namespace GtKanu.Infrastructure.User;

public static class UserClaims
{
    // Authentication Method Reference (amr)
    public static readonly Claim TwoFactorClaim = new("amr", "mfa");
}
