namespace GtKanu.Application.Services;

public interface IEmailService
{
    Task<bool> EnqueueConfirmRegistration(string name, string email, string callbackUrl, bool isExtended, CancellationToken cancellationToken);
    Task<bool> EnqueueChangeEmail(string name, string newEmail, string callbackUrl, CancellationToken cancellationToken);
    Task<bool> EnqueueChangePassword(string name, string email, string callbackUrl, CancellationToken cancellationToken);
    Task<bool> EnqueMailing(Guid id, CancellationToken cancellationToken);
    Task HandleEmails(CancellationToken cancellationToken);
}
