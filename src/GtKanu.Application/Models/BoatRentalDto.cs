using GtKanu.Application.Converter;

namespace GtKanu.Application.Models;

public sealed class BoatRentalDto
{
    public Guid Id { get; set; }
    public Guid BoatId { get; set; }
    public Guid UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? User { get; set; }
    public string? Purpose { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool IsFinished { get; set; }
    public bool IsCancelled { get; set; }      
}
