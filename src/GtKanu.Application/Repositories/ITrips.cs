using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface ITrips
{
    Task<bool> CreateTrip(TripDto dto, CancellationToken cancellationToken);
    Task<TripDto?> FindTrip(Guid id, CancellationToken cancellationToken);
    Task<TripBookingDto[]> GetBookingList(Guid tripId, CancellationToken cancellationToken);
    Task<TripListDto[]> GetTripList(bool showExpired, CancellationToken cancellationToken);
    Task<TripListDto?> FindTripList(Guid id, CancellationToken cancellationToken);
    Task<MyTripListDto[]> GetMyTripList(Guid userId, CancellationToken cancellationToken);
    Task<TripBookingStatus> CreateBooking(Guid id, Guid userId, string? name, CancellationToken cancellationToken);
    Task<bool> DeleteBooking(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<bool> ConfirmBooking(Guid id, CancellationToken cancellationToken);
    Task<bool> CancelBooking(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateTrip(TripDto dto, CancellationToken cancellationToken);
    Task<TripChatDto[]> GetChat(Guid tripId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CreateChatMessage(Guid id, Guid userId, string message, CancellationToken cancellationToken);
    Task<bool> DeleteTrip(Guid id, CancellationToken cancellationToken);
    Task<PublicTripDto[]> GetPublicTrips(CancellationToken cancellationToken);
    Task<string> GetPublicTripsAsIcs(CancellationToken cancellationToken);
}
