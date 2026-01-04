namespace GtKanu.Application.Models;

public sealed class BoatDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Identifier { get; set; }
    public int MaxRentalDays { get; set; }
    public bool IsLocked { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string RentalDetails => MaxRentalDays == 0 ? $"Langzeitmiete" : $"max. {MaxRentalDays} Tag(e) mieten";
    public string NameDetails => $"{Name} #{Identifier}";
    public string FullDetails => NameDetails + ", " + RentalDetails;
}
