using GtKanu.Application.Models;

namespace GtKanu.WebApp.Converter;

public class RoleConverter
{
    public string RoleToClass(IEnumerable<string>? roles)
    {
        if (roles == null) return string.Empty;
        if (roles.Any(r => r == Roles.Admin)) return "has-text-danger";

        if (roles.Any(r => r == Roles.Treasurer)) return "has-text-warning";
        if (roles.Any(r => r == Roles.Chairperson)) return "has-text-warning";
        if (roles.Any(r => r == Roles.UserManager)) return "has-text-warning";

        if (roles.Any(r => r == Roles.FleetManager)) return "has-text-info";
        if (roles.Any(r => r == Roles.Kitchen)) return "has-text-info";
        if (roles.Any(r => r == Roles.TripManager)) return "has-text-info";
        if (roles.Any(r => r == Roles.BoatManager)) return "has-text-info";
        if (roles.Any(r => r == Roles.HouseManager)) return "has-text-info";
        if (roles.Any(r => r == Roles.MailingManager)) return "has-text-info";
        if (roles.Any(r => r == Roles.WikiManager)) return "has-text-info";

        if (roles.Any(r => r == Roles.Member)) return "has-text-success";
        return string.Empty;
    }

    public string RoleToName(string role)
    {
        return role switch
        {
            Roles.Admin => "Administrator",
            Roles.Treasurer => "Kassenwart",
            Roles.Kitchen => "Küchendienst",
            Roles.Member => "Mitglied",
            Roles.Interested => "Interessent",
            Roles.TripManager => "Fahrtenplaner",
            Roles.Chairperson => "Vorsizende/r",
            Roles.UserManager => "Benutzermanager",
            Roles.FleetManager => "Fahrzeugwart",
            Roles.BoatManager => "Bootswart",
            Roles.HouseManager => "Hauswart",
            Roles.MailingManager => "Mailingmanager",
            Roles.WikiManager => "Wikimanager",
            _ => string.Empty
        };
    }
}
