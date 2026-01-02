namespace GtKanu.Core.Entities;

public sealed class Boat
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Identifier { get; set; }
    public string? Location { get; set; }
    public int MaxRentalDays { get; set; }
    public bool IsLocked { get; set; }
    public string? Description { get; set; }

    internal ICollection<BoatRental>? BoatRentals { get; set; }
}
