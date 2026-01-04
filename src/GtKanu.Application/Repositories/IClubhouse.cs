using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IClubhouse
{
    Task<ClubhouseBookingStatus> CreateBooking(ClubhouseBookingDto dto, CancellationToken cancellationToken);
    Task<ClubhouseBookingDto?> FindBooking(Guid id, CancellationToken cancellationToken);
    Task<ClubhouseBookingStatus> UpdateBooking(ClubhouseBookingDto dto, CancellationToken cancellationToken);
    Task<ClubhouseBookingDto[]> GetBookingList(bool showExpired, CancellationToken cancellationToken);
    Task<bool> DeleteBooking(Guid id, CancellationToken cancellationToken);
}
