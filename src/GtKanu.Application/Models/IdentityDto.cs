namespace GtKanu.Application.Models;

public struct IdentityDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? FirstName { get; set; }
    public string? Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTimeOffset? LastLogin { get; set; }
    public string[]? Roles { get; set; }
    public string? DebtorNumber { get; set; }
    public string? AddressNumber { get; set; }
    public bool IsLocked { get; set; }
    public bool IsEnabledForBookings =>
        !(string.IsNullOrWhiteSpace(DebtorNumber) && string.IsNullOrWhiteSpace(AddressNumber));
    public string[]? Mailings { get; set; }
}
