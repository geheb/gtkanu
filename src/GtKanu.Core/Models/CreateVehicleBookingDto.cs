namespace GtKanu.Core.Models;

public sealed class CreateVehicleBookingDto
{
    public Guid VehicleId { get; set; }
    public Guid UserId { get; set; }
    public string? Purpose { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
