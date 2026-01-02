using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public struct MyMailingDto : IDto
{
    public Guid Id { get; set; }
    public Guid? MailingId { get; set; }
    public Guid? UserId { get; set; }
    public DateTimeOffset Created { get; set; }
    public string? ReplyAddress { get; set; }
    public string? Subject { get; set; }
    public string? HtmlBody { get; set; }
    public bool HasRead { get; set; }
}
