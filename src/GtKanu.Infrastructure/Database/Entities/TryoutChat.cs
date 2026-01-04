using GtKanu.Application.Converter;
using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class TryoutChat
{
    public Guid Id { get; set; }
    public Guid? TryoutId { get; set; }
    public Tryout? Tryout { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? Message { get; set; }

    internal TryoutChatDto ToDto(Guid userId, GermanDateTimeConverter dc) => new()
    {
        User = userId == UserId ? null : User?.Name,
        CreatedOn = dc.ToLocal(CreatedOn),
        Message = Message,
    };
}
