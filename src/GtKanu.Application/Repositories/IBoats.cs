using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IBoats
{
    Task<BoatStatus> Create(BoatDto dto, CancellationToken cancellationToken);
    Task<BoatStatus> Update(BoatDto dto, CancellationToken cancellationToken);
    Task<BoatDto?> FindBoat(Guid id, CancellationToken cancellationToken);
    Task<BoatRentalListDto[]> GetRentalList(CancellationToken cancellationToken);
    Task<BoatRentalListDto[]> GetMyRentalList(Guid userId, CancellationToken cancellationToken);
    Task<BoatRentalDto[]> GetRentals(Guid boatId, bool activeOnly, CancellationToken cancellationToken);
    Task<BoatRentalDto?> GetLastRental(Guid boatId, CancellationToken cancellationToken);
    Task<BoatRentalStatus> CreateRental(CreateBoatRentalDto dto, CancellationToken cancellationToken);
    Task<bool> StopRental(Guid id, CancellationToken cancellationToken);
    Task<bool> CancelRental(Guid id, CancellationToken cancellationToken);
    Task<MyBoatRentalListDto[]> GetMyRentals(Guid userId, CancellationToken cancellationToken);
}
