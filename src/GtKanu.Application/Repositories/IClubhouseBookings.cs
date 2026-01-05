using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IClubhouseBookings
{
    Task<ClubhouseBookingStatus> CreateBooking(ClubhouseBookingDto dto, CancellationToken cancellationToken);
    Task<ClubhouseBookingDto?> FindBooking(Guid id, CancellationToken cancellationToken);
    Task<ClubhouseBookingStatus> UpdateBooking(ClubhouseBookingDto dto, CancellationToken cancellationToken);
    Task<ClubhouseBookingDto[]> GetAll(bool showExpired, CancellationToken cancellationToken);
    Task<bool> DeleteBooking(Guid id, CancellationToken cancellationToken);
}
