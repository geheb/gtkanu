using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class ClubhouseBooking
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    [NotMapped]
    public bool IsExpired => End < DateTimeOffset.UtcNow;

    internal ClubhouseBookingDto ToDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        Start = dc.ToLocal(Start),
        End = dc.ToLocal(End),
        Title = Title,
        Description = Description,
        IsExpired = IsExpired,
    };

    internal void FromDto(ClubhouseBookingDto dto)
    {
        Id = dto.Id;
        Start = dto.Start;
        End = dto.End;
        Title = dto.Title?.Trim();
        Description = Description?.Trim();
    }
}
