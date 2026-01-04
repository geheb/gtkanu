namespace GtKanu.Infrastructure.Database.Entities; 

using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using System;

internal sealed class TripChat
{
    public Guid Id { get; set; }
    public Guid? TripId { get; set; }
    public Trip? Trip { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? Message { get; set; }

    internal TripChatDto ToDto(Guid userId, GermanDateTimeConverter dc) => new()
    {
        User = userId == UserId ? null : User?.Name,
        CreatedOn = dc.ToLocal(CreatedOn),
        Message = Message,
    };
}
