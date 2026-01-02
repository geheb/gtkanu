using GtKanu.Core.Converter;
using GtKanu.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace GtKanu.Core.Entities
{
    public sealed class IdentityUserGuid : IdentityUser<Guid>
    {
        public string? Name { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public DateTimeOffset? LeftOn { get; set; }
        public string? DebtorNumber { get; set; }
        public string? AddressNumber { get; set; }
        public string? AuthenticatorKey { get; set; }
        public string? Mailings { get; set; }

        internal ICollection<IdentityUserRoleGuid>? UserRoles { get; set; }
        internal ICollection<Booking>? Bookings { get; set; }
        internal ICollection<Invoice>? Invoices { get; set; }
        internal ICollection<Trip>? Trips { get; set; }
        internal ICollection<TripBooking>? TripBookings { get; set; }
        internal ICollection<WikiArticle>? WikiArticles { get; set; }
        internal ICollection<TripChat>? TripChats { get; set; }
        internal ICollection<VehicleBooking>? VehicleBookings { get; set; }
        internal ICollection<Tryout>? Tryouts { get; set; }
        internal ICollection<TryoutBooking>? TryoutBookings {  get; set; }
        internal ICollection<BoatRental>? BoatRentals { get; set; }
        internal ICollection<TryoutChat>? TryoutChats { get; set; }
        internal ICollection<MyMailing>? MyMailings { get; set; }

        public IdentityDto ToDto(GermanDateTimeConverter dc)
        {
            var name = Name?.Trim() ?? string.Empty;
            var index = name.IndexOf(' ');
            var surname = index > 0 ? name[(index + 1)..].TrimStart() : name;
            var firstName = index > 0 ? name[..index] : name;

            return new()
            {
                Id = Id,
                Name = Name,
                Surname = surname,
                FirstName = firstName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                IsEmailConfirmed = EmailConfirmed,
                LastLogin = LastLogin.HasValue ? dc.ToLocal(LastLogin.Value) : null,
                Roles = UserRoles?.Where(e => e.Role?.Name != null).Select(e => e.Role!.Name!).ToArray(),
                DebtorNumber = DebtorNumber,
                AddressNumber = AddressNumber,
                IsLocked = LockoutEnabled && LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow,
                Mailings = Mailings?.Split(','),
            };
        }
    }
}
