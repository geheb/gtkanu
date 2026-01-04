using FluentResults;
using GtKanu.Application.Models;

namespace GtKanu.Application.Services;

public interface IUserService
{
    Task<Result> UpdatePassword(Guid id, string newPassword);
    Task<Result> ChangePassword(Guid id, string currentPassword, string newPassword);
    Task<Result> VerifyPassword(Guid id, string currentPassword);
    Task<Result> VerifyChangePassword(Guid id, string token);
    Task<Result> ConfirmChangePassword(Guid id, string token, string newPassword);
    Task<Result> ConfirmChangeEmail(Guid id, string token, string newEmail);
    Task<Result> VerifyConfirmRegistration(Guid id, string token);
    Task<Result> ConfirmRegistration(Guid id, string token);
    Task<Result> NotifyConfirmRegistration(string email, string callbackUrl, CancellationToken cancellationToken);
    Task<Result> ReNotifyConfirmRegistration(Guid id, string callbackUrl, CancellationToken cancellationToken);
    Task<Result> NotifyChangePassword(string email, string callbackUrl, CancellationToken cancellationToken);
    Task<Result> NotifyChangeEmail(Guid id, string newEmail, string callbackUrl, CancellationToken cancellationToken);
    Task<Result<UserTwoFactor>> CreateTwoFactor(Guid id, string appName);
    Task<Result> EnableTwoFactor(Guid id, bool enable, string code);
    Task<Result> ResetTwoFactor(Guid id);
}
