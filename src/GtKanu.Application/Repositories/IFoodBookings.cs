using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IFoodBookings
{
    Task<BookingFoodDto[]> GetUntilEndOfMonth(DateTime start, CancellationToken cancellationToken);
    Task<BookingFoodDto[]> GetForOneMonth(Guid userId, int year, int month, CancellationToken cancellationToken);
    Task<decimal> CalcTotal(DateTime from, DateTime to, CancellationToken cancellationToken);
    Task<BookingFoodDto[]> GetNotCancelledForOneDay(DateTime date, CancellationToken cancellationToken);
    Task<BookingFoodDto[]> GetInvoiceBookings(Guid invoiceId, CancellationToken cancellationToken);
    Task<bool> Cancel(Guid userId, Guid bookingId, CancellationToken cancellationToken);
    Task<bool> Complete(Guid bookingId, CancellationToken cancellationToken);
    Task<bool> Create(Guid userId, Guid? foodId, int count, CancellationToken cancellationToken);
}
