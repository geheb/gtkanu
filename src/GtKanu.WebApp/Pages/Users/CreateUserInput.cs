using GtKanu.Application.Models;
using GtKanu.Infrastructure.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Users;

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
        if (Roles[0]) roles.Add(Application.Models.Roles.Admin);
        if (Roles[1]) roles.Add(Application.Models.Roles.Treasurer);
        if (Roles[2]) roles.Add(Application.Models.Roles.Kitchen);
        if (Roles[3]) roles.Add(Application.Models.Roles.Member);
        if (Roles[4]) roles.Add(Application.Models.Roles.Interested);
        if (Roles[5]) roles.Add(Application.Models.Roles.TripManager);
        if (Roles[6]) roles.Add(Application.Models.Roles.Chairperson);
        if (Roles[7]) roles.Add(Application.Models.Roles.UserManager);
        if (Roles[8]) roles.Add(Application.Models.Roles.FleetManager);
        if (Roles[9]) roles.Add(Application.Models.Roles.BoatManager);
        if (Roles[10]) roles.Add(Application.Models.Roles.HouseManager);
        if (Roles[11]) roles.Add(Application.Models.Roles.MailingManager);
        if (Roles[12]) roles.Add(Application.Models.Roles.WikiManager);

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
