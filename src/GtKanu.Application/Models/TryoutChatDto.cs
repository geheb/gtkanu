namespace GtKanu.Application.Models;

using System;

public sealed class TryoutChatDto
{
    public string? User { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? Message { get; set; }
}
