using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public struct EmailQueueDto : IDto
{
    public Guid Id { get; set; }

    public string? Recipient { get; set; }

    public string? ReplyAddress { get; set; }

    public string? Subject { get; set; }

    public string? HtmlBody { get; set; }

    public bool IsPrio { get; set; }

    public EmailItem ToEmailItem() => new()
    {
        Subject = Subject!,
        HtmlBody = HtmlBody!,
        Recipient = Recipient!,
        ReplyAddress = ReplyAddress,
    };
}
