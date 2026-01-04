namespace GtKanu.Application.Models;

public sealed class MyBoatRentalListDto
{
    public string? Purpose { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool IsFinished { get; set; }
    public bool IsCancelled { get; set; }
}
