namespace GtKanu.Application.Models;

public struct EmailItem
{
    public string Recipient { get; set; }
    public string Subject { get; set; }
    public string HtmlBody { get; set; }
    public string? ReplyAddress { get; set; }
}
