using GtKanu.Core.Converter;
using GtKanu.Core.Entities;

namespace GtKanu.Core.Models;

public sealed class TryoutListDto
{
    public Guid Id { get; set; }
    public string? Type { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public int BookingCount { get; }
    public int MaxBookings { get; }
    public bool IsExpired { get; }
    public string? Description { get; set; }
    public bool CanAccept { get; }
    public int ChatMessageCount { get; }
    public string[] BookingUsers { get; }

    internal TryoutListDto(Tryout tryout, int bookingCount, int chatMessageCount, string[] bookingUsers, GermanDateTimeConverter dc)
    {
        Id = tryout.Id;
        Type = tryout.Type;
        Date = dc.ToLocal(tryout.Date);
        ContactName = tryout.User?.Name;
        ContactEmail = tryout.User?.EmailConfirmed == true ? tryout.User.Email : null;
        BookingCount = bookingCount;
        ChatMessageCount = chatMessageCount;
        BookingUsers = bookingUsers;

        MaxBookings = tryout.MaxBookings;
        IsExpired = tryout.IsExpired;

        Description = tryout.Description;

        var canBook =
            DateTimeOffset.UtcNow >= tryout.BookingStart &&
            DateTimeOffset.UtcNow <= tryout.BookingEnd;

        CanAccept = canBook && bookingCount < tryout.MaxBookings;
    }
}
