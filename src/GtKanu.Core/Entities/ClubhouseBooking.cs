using System.ComponentModel.DataAnnotations.Schema;

namespace GtKanu.Core.Entities;

internal sealed class ClubhouseBooking
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    [NotMapped]
    public bool IsExpired => End < DateTimeOffset.UtcNow;
}
