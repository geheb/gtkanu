using GtKanu.Core.Entities;
using GtKanu.Ui.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.Ui.Pages.Users;

public class CreateUserInput
{
    [Display(Name = "Name")]
    [RequiredField, TextLengthField]
    public string? Name { get; set; }

    [Display(Name = "E-Mail-Adresse")]
    [RequiredField, EmailLengthField, EmailField]
    public string? Email { get; set; }

    [Display(Name = "Telefonnummer")]
    [PhoneField]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Rollen")]
    [RequiredField]
    public bool[] Roles { get; set; } = new bool[13];

    [Display(Name = "Debitoren-Nr.")]
    [TextLengthField]
    public string? DebtorNumber { get; set; }

    [Display(Name = "Adress-Nr.")]
    [TextLengthField]
    public string? AddressNumber { get; set; }

    [Display(Name = "Jugend")]
    public bool IsMailingYoungPeople { get; set; }

    public IdentityDto ToDto()
    {
        var roles = new List<string>();
        if (Roles[0]) roles.Add(Core.Models.Roles.Admin);
        if (Roles[1]) roles.Add(Core.Models.Roles.Treasurer);
        if (Roles[2]) roles.Add(Core.Models.Roles.Kitchen);
        if (Roles[3]) roles.Add(Core.Models.Roles.Member);
        if (Roles[4]) roles.Add(Core.Models.Roles.Interested);
        if (Roles[5]) roles.Add(Core.Models.Roles.TripManager);
        if (Roles[6]) roles.Add(Core.Models.Roles.Chairperson);
        if (Roles[7]) roles.Add(Core.Models.Roles.UserManager);
        if (Roles[8]) roles.Add(Core.Models.Roles.FleetManager);
        if (Roles[9]) roles.Add(Core.Models.Roles.BoatManager);
        if (Roles[10]) roles.Add(Core.Models.Roles.HouseManager);
        if (Roles[11]) roles.Add(Core.Models.Roles.MailingManager);
        if (Roles[12]) roles.Add(Core.Models.Roles.WikiManager);

        return new()
        {
            Name = Name,
            Email = Email,
            PhoneNumber = PhoneNumber,
            Roles = roles.ToArray(),
            DebtorNumber = DebtorNumber,
            AddressNumber = AddressNumber,
            Mailings = IsMailingYoungPeople ? [UserMailings.YoungPeople] : [],
        };
    }
}
