using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace GtKanu.Infrastructure.Email;

internal sealed class EmailService : IEmailService
{
    private readonly EmailTemplateRenderer _emailTemplateRenderer = new();
    private readonly TimeSpan _confirmEmailTimeout, _changeEmailPassTimeout;
    private readonly SmtpDispatcher _smtpDispatcher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentities _identityRepository;

    public EmailService(
        IOptions<ConfirmEmailDataProtectionTokenProviderOptions> confirmEmailOptions,
        IOptions<DataProtectionTokenProviderOptions> changeEmailPassOptions,
        SmtpDispatcher smtpDispatcher,
        IUnitOfWork unitOfWork,
        IIdentities identityRepository)
    {
        _confirmEmailTimeout = confirmEmailOptions.Value.TokenLifespan;
        _changeEmailPassTimeout = changeEmailPassOptions.Value.TokenLifespan;
        _smtpDispatcher = smtpDispatcher;
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
    }

    public async Task<bool> EnqueueConfirmRegistration(string name, string email, string callbackUrl, bool isExtended, CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();

        var model = new
        {
            title = "GT Kanu - Registrierung",
            name = name.Split(' ')[0],
            link = callbackUrl,
            timeout = dc.Format(_confirmEmailTimeout)
        };

        var template = isExtended ? EmailTemplate.ConfirmRegistrationExtended : EmailTemplate.ConfirmRegistration;

        var htmlBody = _emailTemplateRenderer.Render(template, model);

        var dto = new EmailQueueDto
        {
            Recipient = email,
            Subject = model.title,
            HtmlBody = htmlBody,
            IsPrio = true,
        };

        _unitOfWork.EmailQueue.Create(dto);
        return await _unitOfWork.Save(cancellationToken) > 0;
    }

    public async Task<bool> EnqueueChangeEmail(string name, string newEmail, string callbackUrl, CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();

        var model = new
        {
            title = "GT Kanu - E-Mail-Adresse",
            name = name.Split(' ')[0],
            link = callbackUrl,
            timeout = dc.Format(_changeEmailPassTimeout)
        };

        var htmlBody = _emailTemplateRenderer.Render(EmailTemplate.ConfirmChangeEmail, model);

        var dto = new EmailQueueDto
        {
            Recipient = newEmail,
            Subject = model.title,
            HtmlBody = htmlBody,
            IsPrio = true,
        };

        _unitOfWork.EmailQueue.Create(dto);
        return await _unitOfWork.Save(cancellationToken) > 0;
    }

    public async Task<bool> EnqueueChangePassword(string name, string email, string callbackUrl, CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();

        var model = new
        {
            title = "GT Kanu - Passwort",
            name = name.Split(' ')[0],
            link = callbackUrl,
            timeout = dc.Format(_changeEmailPassTimeout)
        };

        var htmlBody = _emailTemplateRenderer.Render(EmailTemplate.ConfirmPasswordForgotten, model);

        var dto = new EmailQueueDto
        {
            Recipient = email,
            Subject = model.title,
            HtmlBody = htmlBody,
            IsPrio = true,
        };

        _unitOfWork.EmailQueue.Create(dto);
        return await _unitOfWork.Save(cancellationToken) > 0;
    }

    public async Task<bool> EnqueMailing(Guid id, CancellationToken cancellationToken)
    {
        var mailing = await _unitOfWork.Mailings.Find(id, cancellationToken);
        if (mailing is null)
        {
            return false;
        }

        var templateModel = new
        {
            title = mailing.Value.Subject,
            body = mailing.Value.Body,
        };

        var htmlBody = _emailTemplateRenderer.Render(EmailTemplate.Mailing, templateModel);

        var users = await _identityRepository.GetAll(cancellationToken);

        var recipients = new Dictionary<string, IdentityDto?>(StringComparer.OrdinalIgnoreCase);
        if (mailing.Value.IsMemberOnly)
        {
            foreach (var u in users.Where(u => u.Roles!.Any(r => r == Roles.Member)))
            {
                recipients.Add(u.Email!, u);
            }
        }
        if (mailing.Value.IsYoungPeople)
        {
            foreach (var u in users.Where(u => u.Mailings?.Any(m => m == UserMailings.YoungPeople) == true))
            {
                recipients.TryAdd(u.Email!, u);
            }
        }

        if (mailing.Value.OtherRecipients?.Length > 0)
        {
            foreach (var recipient in mailing.Value.OtherRecipients)
            {
                var index = Array.FindIndex(users, u => recipient.Equals(u.Email, StringComparison.OrdinalIgnoreCase));
                recipients.TryAdd(recipient, index < 0 ? null : users[index]);
            }
        }

        foreach (var (email, user) in recipients)
        {
            var emailQueue = new EmailQueueDto
            {
                Recipient = email,
                ReplyAddress = mailing.Value.ReplyAddress,
                Subject = mailing.Value.Subject,
                HtmlBody = htmlBody,
                CorrelationId = mailing.Value.Id,
            };
            _unitOfWork.EmailQueue.Create(emailQueue);

            if (user is not null)
            {
                var myMailing = new MyMailingDto
                {
                    MailingId = mailing.Value.Id,
                    UserId = user.Value.Id
                };
                _unitOfWork.MyMailings.Create(myMailing);
            }
        }

        var result = await _unitOfWork.Mailings.UpdateClosed(mailing.Value.Id, recipients.Count, cancellationToken);
        if (result.IsFailed)
        {
            return false;
        }

        if (await _unitOfWork.Save(cancellationToken) < 1)
        {
            return false;
        }

        return true;
    }

    public async Task HandleEmails(CancellationToken cancellationToken)
    {
        var emailQueue = await _unitOfWork.EmailQueue.GetNextToSend(10, cancellationToken);
        if (emailQueue.Length == 0)
        {
            return;
        }

        var items = emailQueue.Select(e => e.ToEmailItem()).ToArray();

        try
        {
            await _smtpDispatcher.Send(items, cancellationToken);

            await _unitOfWork.EmailQueue.UpdateSent([.. emailQueue.Select(e => e.Id)], cancellationToken);
            await _unitOfWork.Save(cancellationToken);
        }
        catch (SmtpException)
        {
            await ScheduleEmails(emailQueue, cancellationToken);
        }
    }

    private async Task ScheduleEmails(EmailQueueDto[] queue, CancellationToken cancellationToken)
    {
        foreach (var q in queue)
        {
            var item = q.ToEmailItem();

            try
            {
                await _smtpDispatcher.Send([item], cancellationToken);
                await _unitOfWork.EmailQueue.UpdateSent([q.Id], cancellationToken);
            }
            catch (SmtpException ex)
            {
                await _unitOfWork.EmailQueue.UpdateNextSchedule(q.Id, ex.InnerException?.Message ?? ex.Message, cancellationToken);
            }

            await _unitOfWork.Save(cancellationToken);
        }
    }
}
