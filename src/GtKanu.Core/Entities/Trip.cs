using System.ComponentModel.DataAnnotations.Schema;

namespace GtKanu.Core.Entities;

internal class Trip
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
}
