using FluentResults;
using GtKanu.Application.Models;
using GtKanu.Application.Services;
using GtKanu.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Web;

namespace GtKanu.Infrastructure.User;

internal sealed class UserService : IUserService
{
    private readonly Result _userNotFound = Result.Fail("Benutzer wurde nicht gefunden.");
    private readonly UserManager<IdentityUserGuid> _userManager;
    private readonly IEmailService _emailService;

    public UserService(
        UserManager<IdentityUserGuid> userManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<Result> UpdatePassword(Guid id, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(r => r.Description));
        }

        return Result.Ok();
    }

    public async Task<Result> ChangePassword(Guid id, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, currentPassword);
        if (result != PasswordVerificationResult.Success)
        {
            return Result.Fail("Das aktuelle Passwort ist ungültig.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var identityResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!identityResult.Succeeded)
        {
            return Result.Fail(identityResult.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }


    public async Task<Result> VerifyPassword(Guid id, string currentPassword)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, currentPassword);
        if (result != PasswordVerificationResult.Success)
        {
            return Result.Fail(_userManager.ErrorDescriber.PasswordMismatch().Description);
        }

        return Result.Ok();
    }

    public async Task<Result> VerifyChangePassword(Guid id, string token)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        var isValid = await _userManager.VerifyUserTokenAsync(user,
            _userManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<IdentityUserGuid>.ResetPasswordTokenPurpose,
            token);

        if (!isValid)
        {
            return Result.Fail(_userManager.ErrorDescriber.InvalidToken().Description);
        }

        return Result.Ok();
    }

    public async Task<Result> ConfirmChangePassword(Guid id, string token, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(r => r.Description));
        }

        return Result.Ok();
    }

    public async Task<Result> ConfirmChangeEmail(Guid id, string token, string newEmail)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    public async Task<Result> VerifyConfirmRegistration(Guid id, string token)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        var isValid = await _userManager.VerifyUserTokenAsync(user,
            _userManager.Options.Tokens.EmailConfirmationTokenProvider,
            UserManager<IdentityUserGuid>.ConfirmEmailTokenPurpose,
            token);

        if (!isValid)
        {
            return Result.Fail(_userManager.ErrorDescriber.InvalidToken().Description);
        }

        return Result.Ok();
    }

    public async Task<Result> ConfirmRegistration(Guid id, string token)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(r => r.Description));
        }

        return Result.Ok();
    }

    public async Task<Result> NotifyConfirmRegistration(string email, string callbackUrl, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return _userNotFound;
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var uriBuilder = new UriBuilder(callbackUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["id"] = user.Id.ToString();
        query["token"] = token;
        uriBuilder.Query = query.ToString();

        var result = await _emailService.EnqueueConfirmRegistration(user.Name!, user.Email!, uriBuilder.ToString(), true, cancellationToken);
        if (!result)
        {
            return Result.Fail("Interner Server Fehler.");
        }

        return Result.Ok();
    }

    public async Task<Result> ReNotifyConfirmRegistration(Guid id, string callbackUrl, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var uriBuilder = new UriBuilder(callbackUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["id"] = user.Id.ToString();
        query["token"] = token;
        uriBuilder.Query = query.ToString();

        var result = await _emailService.EnqueueConfirmRegistration(user.Name!, user.Email!, uriBuilder.ToString(), false, cancellationToken);
        if (!result)
        {
            return Result.Fail("Interner Server Fehler.");
        }

        return Result.Ok();
    }

    public async Task<Result> NotifyChangePassword(string email, string callbackUrl, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return _userNotFound;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var uriBuilder = new UriBuilder(callbackUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["id"] = user.Id.ToString();
        query["token"] = token;
        uriBuilder.Query = query.ToString();

        var result = await _emailService.EnqueueChangePassword(user.Name!, user.Email!, uriBuilder.ToString(), cancellationToken);
        if (!result)
        {
            return Result.Fail("Interner Server Fehler.");
        }

        return Result.Ok();
    }

    public async Task<Result> NotifyChangeEmail(Guid id, string newEmail, string callbackUrl, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        if (await _userManager.FindByEmailAsync(newEmail) is not null)
        {
            return Result.Fail(_userManager.ErrorDescriber.DuplicateEmail(newEmail).Description);
        }

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

        var uriBuilder = new UriBuilder(callbackUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["id"] = user.Id.ToString();
        query["token"] = token;
        query["email"] = newEmail;
        uriBuilder.Query = query.ToString();

        var result = await _emailService.EnqueueChangeEmail(user.Name!, newEmail, uriBuilder.ToString(), cancellationToken);
        if (!result)
        {
            return Result.Fail("Interner Server Fehler.");
        }

        return Result.Ok();
    }

    public async Task<Result<UserTwoFactor>> CreateTwoFactor(Guid id, string appName)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            var result = await _userManager.ResetAuthenticatorKeyAsync(user);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(e => e.Description));
            }
            key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                return Result.Fail(_userManager.ErrorDescriber.DefaultError().Description);
            }
        }

        var uri = GenerateOtpAuthUri(appName, user.Email!, key);
        return Result.Ok(new UserTwoFactor(isTwoFactorEnabled, key, uri));
    }

    public async Task<Result> EnableTwoFactor(Guid id, bool enable, string code)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider, code);

        if (!isValid)
        {
            return Result.Fail(_userManager.ErrorDescriber.InvalidToken().Description);
        }

        var result = await _userManager.SetTwoFactorEnabledAsync(user, enable);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        if (enable)
        {
            result = await _userManager.AddClaimAsync(user, UserClaims.TwoFactorClaim);
        }
        else
        {
            result = await _userManager.RemoveClaimAsync(user, UserClaims.TwoFactorClaim);
        }

        if (!result.Succeeded)
        {
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    public async Task<Result> ResetTwoFactor(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return _userNotFound;
        }

        user.AuthenticatorKey = null;
        var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        result = await _userManager.RemoveClaimAsync(user, UserClaims.TwoFactorClaim);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors.Select(e => e.Description));
        }

        return Result.Ok();
    }

    private static string GenerateOtpAuthUri(string issuer, string user, string secret)
    {
        var dictionary = new Dictionary<string, string>
        {
            { "secret", secret },
            { "issuer", Uri.EscapeDataString(issuer) },
            { "algorithm","SHA1" },
            { "digits", "6" },
            { "period", "30" }
        };

        var uri = new StringBuilder("otpauth://totp/");
        uri.Append(Uri.EscapeDataString(issuer));
        uri.Append(':');
        uri.Append(Uri.EscapeDataString(user));
        uri.Append('?');
        foreach (var item in dictionary)
        {
            uri.Append(item.Key);
            uri.Append('=');
            uri.Append(item.Value);
            uri.Append('&');
        }

        // remove '&' at the end
        uri.Remove(uri.Length - 1, 1);
        return uri.ToString();
    }
}
