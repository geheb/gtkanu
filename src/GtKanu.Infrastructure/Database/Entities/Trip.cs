using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class Trip
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public string? Target { get; set; }
    public int MaxBookings { get; set; }
    public DateTimeOffset? BookingStart { get; set; }
    public DateTimeOffset? BookingEnd { get; set; }
    public string? Description { get; set; }

    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow > End;

    public int Categories { get; set; }
    public bool IsPublic { get; set; }

    public ICollection<TripBooking>? TripBookings { get; set; }
    public ICollection<TripChat>? TripChats { get; set; }


    internal TripDto ToDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        Start = dc.ToLocal(Start),
        End = dc.ToLocal(End),
        Target = Target,
        UserId = UserId!.Value,
        MaxBookings = MaxBookings,
        BookingStart = dc.ToLocal(BookingStart!.Value),
        BookingEnd = dc.ToLocal(BookingEnd!.Value),
        Description = Description,
        Categories = (TripCategory)Categories,
        IsPublic = IsPublic,
    };

    internal void FromDto(TripDto dto)
    {
        Start = dto.Start;
        End = dto.End;
        Target = dto.Target?.Trim();
        UserId = dto.UserId;
        MaxBookings = dto.MaxBookings;
        BookingStart = dto.BookingStart;
        BookingEnd = dto.BookingEnd;
        Description = dto.Description?.Trim();
        Categories = (int)dto.Categories;
        IsPublic = dto.IsPublic;
    }

    internal PublicTripDto ToPublicDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        Start = dc.ToLocal(Start),
        End = dc.ToLocal(End),
        Target = Target,
        Categories = (TripCategory)Categories
    };

    internal TripListDto ToListDto(int bookingCount, int chatMessageCount, string[] bookingUsers, GermanDateTimeConverter dc)
    {
        var canBook =
            DateTimeOffset.UtcNow >= BookingStart &&
            DateTimeOffset.UtcNow <= BookingEnd;

        var categories = (TripCategory)Categories;
        string[] categoryNames = [];
        if (categories != TripCategory.None)
        {
            var tripCategoryConverter = new TripCategoryConverter();
            categoryNames = Enum.GetValues<TripCategory>()
                .Where(v => v != TripCategory.None && categories.HasFlag(v))
                .Select(v => tripCategoryConverter.CategoryToName(v))
                .ToArray();
        }

        return new()
        {
            Id = Id,
            Target = Target,
            Start = dc.ToLocal(Start),
            End = dc.ToLocal(End),
            ContactName = User?.Name,
            ContactEmail = User?.EmailConfirmed == true ? User.Email : null,
            BookingCount = bookingCount,
            ChatMessageCount = chatMessageCount,
            MaxBookings = MaxBookings,
            IsExpired = IsExpired,
            Description = Description,
            BookingUsers = bookingUsers,
            CanAccept = canBook && bookingCount < MaxBookings,
            Categories = categoryNames,
        };
    }

    internal MyTripListDto ToMyListDto(TripBooking? booking, int bookingCount, int chatMessageCount, string[] bookingUsers, GermanDateTimeConverter dc)
    {
        var canBook =
            DateTimeOffset.UtcNow >= BookingStart &&
            DateTimeOffset.UtcNow <= BookingEnd;

        var categories = (TripCategory)Categories;
        string[] categoryNames = [];
        if (categories != TripCategory.None)
        {
            var tripCategoryConverter = new TripCategoryConverter();
            categoryNames = Enum.GetValues<TripCategory>()
                .Where(v => v != TripCategory.None && categories.HasFlag(v))
                .Select(v => tripCategoryConverter.CategoryToName(v))
                .ToArray();
        }

        return new()
        {
            TripId = Id,
            BookingId = booking?.Id,
            TripTarget = Target,
            TripStart = dc.ToLocal(Start),
            TripEnd = dc.ToLocal(End),
            TripContactName = User?.Name,
            TripContactEmail = User?.EmailConfirmed == true ? User.Email : null,
            TripDescription = Description,
            MaxBookings = MaxBookings,
            BookingName = booking?.Name ?? User?.Name ?? string.Empty,
            BookingUsers = bookingUsers,
            BookingCount = bookingCount,
            ChatMessageCount = chatMessageCount,
            BookingBookedOn = booking is null ? null : dc.ToLocal(booking.BookedOn),
            BookingConfirmedOn = booking?.ConfirmedOn is not null ? dc.ToLocal(booking.ConfirmedOn.Value) : null,
            BookingCancelledOn = booking?.CancelledOn is not null ? dc.ToLocal(booking.CancelledOn.Value) : null,
            CanAccept = canBook && bookingCount < MaxBookings,
            CanDelete = booking is not null && booking.ConfirmedOn is null,
            IsExpired = IsExpired,
            Categories = categoryNames,
        };
    }
}
