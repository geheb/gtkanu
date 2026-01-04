namespace GtKanu.Application.Models;

public sealed class CreateBoatRentalDto
{
    public Guid BoatId { get; set; }
    public Guid UserId { get; set; }
    public string? Purpose { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
