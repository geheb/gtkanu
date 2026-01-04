namespace GtKanu.Application.Models;

public sealed class InvoiceDto
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? User { get; set; }
    public decimal Total { get; set; }
    public decimal OpenTotal => Status == InvoiceStatus.Open ? Total : 0;
    public decimal PaidTotal => Status == InvoiceStatus.Paid ? Total : 0;
    public InvoiceStatus Status { get; set; }
    public DateTimeOffset? PaidOn { get; set; }
    public string? Description { get; set; }
    public string? Period { get; set; }
}
