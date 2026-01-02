namespace GtKanu.Core.Models;

public static class Roles
{
    public const string Admin = "administrator";
    public const string Treasurer = "treasurer"; // Kassenwart
    public const string Kitchen = "kitchen"; // Küchendienst
    public const string Member = "member"; // Vereinsmitglied
    public const string Interested = "interested"; // Interessent
    public const string TripManager = "tripmanager"; // Fahrtenplaner
    public const string UserManager = "usermanager"; // Benutzermanager
    public const string Chairperson = "chairperson"; // Vorsitzende
    public const string FleetManager = "fleetmanager"; // Fahrzeugwart
    public const string BoatManager = "boatmanager"; // Bootswart
    public const string HouseManager = "housemanager"; // Hauswart
    public const string MailingManager = "mailingmanager"; // Mailingmanager
    public const string WikiManager = "wikimanager"; // Wikimanager

    public static bool IsMemberWithRole(string[] roles) =>
        roles.Any(r => r == Member) &&
        roles.Where(r => r != Member && r != Interested).Any();
}
