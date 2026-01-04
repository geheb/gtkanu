using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IVehicles
{
    Task<VehicleStatus> CreateVehicle(VehicleDto dto, CancellationToken cancellationToken);
    Task<VehicleDto[]> GetAllVehicles(CancellationToken cancellationToken);
    Task<VehicleDto[]> GetVehiclesInUseOnly(CancellationToken cancellationToken);
    Task<VehicleBookingStatus> CreateBooking(CreateVehicleBookingDto dto, CancellationToken cancellationToken);
    Task<VehicleDto?> FindVehicle(Guid id, CancellationToken cancellationToken);
    Task<VehicleStatus> UpdateVehicle(VehicleDto dto, CancellationToken cancellationToken);
    Task<VehicleBookingDto[]> GetBookings(bool showExpired, Guid? userId, CancellationToken cancellationToken);
    Task<bool> DeleteBooking(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<bool> ConfirmBooking(Guid id, CancellationToken cancellationToken);
    Task<bool> CancelBooking(Guid id, CancellationToken cancellationToken);
}
