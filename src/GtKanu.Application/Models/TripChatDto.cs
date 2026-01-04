namespace GtKanu.Application.Models;

using System;

public sealed class TripChatDto
{
    public string? User { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? Message { get; set; }
}
