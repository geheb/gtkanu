using GtKanu.Application.Converter;
using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class EmailQueue : IEntity, IDtoMapper<EmailQueueDto>
{
    public Guid Id { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset? Updated { get; set; }

    public DateTimeOffset? Sent { get; set; }

    public DateTimeOffset? NextSchedule { get; set; }

    public string? Recipient { get; set; }

    public string? Subject { get; set; }

    public string? HtmlBody { get; set; }

    public bool IsPrio { get; set; }

    public string? ReplyAddress { get; set; }

    public string? LastError { get; set; }

    public Guid? CorrelationId { get; set; }

    public void FromDto(EmailQueueDto model)
    {
        Id = model.Id;
        Recipient = model.Recipient;
        Subject = model.Subject;
        HtmlBody = model.HtmlBody;
        IsPrio = model.IsPrio;
        ReplyAddress = model.ReplyAddress;
        CorrelationId = model.CorrelationId;
    }

    public EmailQueueDto ToDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        Recipient = Recipient,
        Subject = Subject,
        HtmlBody = HtmlBody,
        IsPrio = IsPrio,
        ReplyAddress = ReplyAddress,
        CorrelationId = CorrelationId,
    };
}
