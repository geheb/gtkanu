using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public struct MailingDto : IDto
{
    public Guid Id { get; set; }
    public DateTimeOffset LastUpdate { get; set; }
    public bool IsMemberOnly { get; set; }
    public bool IsYoungPeople { get; set; }
    public string[]? OtherRecipients { get; set; }
    public string? ReplyAddress { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public bool IsClosed { get; set; }
    public int EmailCount { get; set; }
}
