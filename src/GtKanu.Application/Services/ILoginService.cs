using FluentResults;

namespace GtKanu.Application.Services;

public interface ILoginService
{
    Task<Result<bool>> SignIn(string email, string password, CancellationToken cancellationToken);
    Task<Result> SignInTwoFactor(string code, bool rememberClient, CancellationToken cancellationToken);
    Task SignOutCurrentUser();
}
