namespace GtKanu.Core.Models;

using GtKanu.Core.Converter;
using GtKanu.Core.Entities;
using System;

public sealed class TryoutChatDto
{
    public string? User { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? Message { get; set; }

    internal TryoutChatDto(TryoutChat entity, Guid userId, GermanDateTimeConverter dc)
    {
        User = userId == entity.UserId ? null : entity.User?.Name;
        CreatedOn = dc.ToLocal(entity.CreatedOn);
        Message = entity.Message;
    }
}
