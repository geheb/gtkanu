using FluentResults;
using GtKanu.Application.Repositories;
using GtKanu.Application.Services;
using GtKanu.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GtKanu.Infrastructure.User;

internal sealed class LoginService : ILoginService
{
    private readonly IIdentities _identityRepository;
    private readonly SignInManager<IdentityUserGuid> _signInManager;
    private readonly int _defaultLockoutMinutes;

    public LoginService(
        IOptions<IdentityOptions> identityOptions,
        SignInManager<IdentityUserGuid> signInManager,
        IIdentities identityRepository)
    {
        _signInManager = signInManager;
        _defaultLockoutMinutes = (int)identityOptions.Value.Lockout.DefaultLockoutTimeSpan.TotalMinutes;
        _identityRepository = identityRepository;
    }

    public async Task<Result<bool>> SignIn(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result.Fail("Login fehlgeschlagen, bitte E-Mail und Passwort überprüfen.");
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            await _identityRepository.UpdateLoginSucceeded(user.Id, cancellationToken);
            return Result.Ok(false);
        }
        else if (result.RequiresTwoFactor)
        {
            return Result.Ok(true); 
        }
        else if (result.IsLockedOut)
        {
            return Result.Fail($"Dein Login ist vorübergehend gesperrt, versuche in {_defaultLockoutMinutes} Minuten wieder.");
        }
        else if (result.IsNotAllowed)
        {
            return Result.Fail("Login fehlgeschlagen, du kannst dich leider noch nicht anmelden.");
        }
        else
        {
            return Result.Fail("Login fehlgeschlagen, bitte E-Mail und Passwort überprüfen.");
        }
    }

    public async Task<Result> SignInTwoFactor(string code, bool rememberClient, CancellationToken cancellationToken)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user is null)
        {
            return Result.Fail("Login fehlgeschlagen, bitte erneut anmelden");
        }
        var signInResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(code, false, rememberClient);
        if (!signInResult.Succeeded)
        {
            return Result.Fail("Login fehlgeschlagen, der Code ist ungültig.");
        }

        await _identityRepository.UpdateLoginSucceeded(user.Id, cancellationToken);

        return Result.Ok();
    }

    public async Task SignOutCurrentUser()
    {
        await _signInManager.SignOutAsync();
    }
}
