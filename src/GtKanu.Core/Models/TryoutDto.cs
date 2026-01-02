namespace GtKanu.Core.Models;

using GtKanu.Core.Converter;
using GtKanu.Core.Entities;

public sealed class TryoutDto
{
    public Guid Id { get; set; }
    public string? Type { get; set; }
    public DateTimeOffset Date { get; set; }
    public Guid UserId { get; set; }
    public int MaxBookings { get; set; }
    public DateTimeOffset BookingStart { get; set; }
    public DateTimeOffset BookingEnd { get; set; }
    public string? Description { get; set; }

    internal TryoutDto(Tryout entity, GermanDateTimeConverter dc)
    {
        Id = entity.Id;
        Type = entity.Type;
        Date = dc.ToLocal(entity.Date);
        UserId = entity.UserId!.Value;
        MaxBookings = entity.MaxBookings;
        BookingStart = dc.ToLocal(entity.BookingStart!.Value);
        BookingEnd = dc.ToLocal(entity.BookingEnd!.Value);
        Description = entity.Description;
    }

    public TryoutDto()
    {
    }

    internal Tryout ToEntity()
    {
        return new()
        {
            Type = Type,
            Date = Date,
            UserId = UserId,
            MaxBookings = MaxBookings,
            BookingStart = BookingStart,
            BookingEnd = BookingEnd,
            Description = Description
        };
    }
}
