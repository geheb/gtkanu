using System.ComponentModel.DataAnnotations.Schema;

namespace GtKanu.Core.Entities;

internal sealed class Tryout
{
    public Guid Id { get; set; }
    public string? Type { get; set; }
    public DateTimeOffset Date { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public int MaxBookings { get; set; }
    public DateTimeOffset? BookingStart { get; set; }
    public DateTimeOffset? BookingEnd { get; set; }
    public string? Description { get; set; }

    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow > Date;

    internal ICollection<TryoutBooking>? TryoutBookings { get; set; }
    internal ICollection<TryoutChat>? TryoutChats { get; set; }
}
