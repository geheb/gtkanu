using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IFoodInvoices
{
    Task<InvoicePeriodDto[]> GetPeriods(CancellationToken cancellationToken);
    Task<InvoiceDto[]> GetByPeriod(Guid id, CancellationToken cancellationToken);
    Task<InvoiceDto[]> GetAll(Guid userId, CancellationToken cancellationToken);
    Task<InvoiceDto?> Find(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<int> Create(DateTime start, DateTime end, string description, CancellationToken cancellationToken);
    Task<bool> UpdateStatusPaid(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateStatusOpen(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateStatusPaidAll(Guid periodId, CancellationToken cancellationToken);
}
