using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface ITryouts
{
    Task<bool> CreateTryout(TryoutDto dto, CancellationToken cancellationToken);
    Task<bool> CreateTryout(TryoutDto[] dtos, CancellationToken cancellationToken);
    Task<TryoutDto?> FindTryout(Guid id, CancellationToken cancellationToken);
    Task<TryoutBookingDto[]> GetBookingList(Guid tryoutId, CancellationToken cancellationToken);
    Task<TryoutListDto[]> GetTryoutList(bool showExpired, bool includeUserList, CancellationToken cancellationToken);
    Task<bool> UpdateTryout(TryoutDto dto, CancellationToken cancellationToken);
    Task<bool> ConfirmBooking(Guid id, CancellationToken cancellationToken);
    Task<bool> CancelBooking(Guid id, CancellationToken cancellationToken);
    Task<bool> DeleteBooking(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<TryoutListDto?> FindTryoutList(Guid id, CancellationToken cancellationToken);
    Task<MyTryoutListDto[]> GetMyTryoutList(Guid userId, bool includeUserList, CancellationToken cancellationToken);
    Task<TryoutBookingStatus> CreateBooking(Guid id, Guid userId, string? name, CancellationToken cancellationToken);
    Task<bool> DeleteTryout(Guid id, CancellationToken cancellationToken);
    Task<TryoutChatDto[]> GetChat(Guid tryoutId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CreateChatMessage(Guid id, Guid userId, string message, CancellationToken cancellationToken);
}
