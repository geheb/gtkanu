using GtKanu.Core.Converter;
using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public sealed class ClubhouseBookingDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsExpired { get; set; }

    public ClubhouseBookingDto()
    {
    }

    internal ClubhouseBookingDto(ClubhouseBooking entity, GermanDateTimeConverter dc)
    {
        Id = entity.Id;
        Start = dc.ToLocal(entity.Start);
        End = dc.ToLocal(entity.End);
        Title = entity.Title;
        Description = entity.Description;
        IsExpired = entity.IsExpired;
    }

    internal ClubhouseBooking ToEntity()
    {
        return new()
        {
            Id = Id,
            Start = Start,
            End = End,
            Title = Title,
            Description = Description
        };
    }
}
