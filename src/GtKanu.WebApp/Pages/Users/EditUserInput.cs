using GtKanu.Application.Models;
using GtKanu.Infrastructure.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Users;

public class EditUserInput
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

    [Display(Name = "Passwort")]
    [PasswordLengthField]
    public string? Password { get; set; }

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

    public void From(IdentityDto dto)
    {
        Name = dto.Name;
        Email = dto.Email;
        PhoneNumber = dto.PhoneNumber;

        if (dto.Roles != null)
        {
            if (dto.Roles.Any(r => r == Application.Models.Roles.Admin)) Roles[0] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.Treasurer)) Roles[1] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.Kitchen)) Roles[2] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.Member)) Roles[3] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.Interested)) Roles[4] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.TripManager)) Roles[5] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.Chairperson)) Roles[6] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.UserManager)) Roles[7] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.FleetManager)) Roles[8] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.BoatManager)) Roles[9] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.HouseManager)) Roles[10] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.MailingManager)) Roles[11] = true;
            if (dto.Roles.Any(r => r == Application.Models.Roles.WikiManager)) Roles[12] = true;
        }
        DebtorNumber = dto.DebtorNumber;
        AddressNumber = dto.AddressNumber;
        IsMailingYoungPeople = dto.Mailings?.Any(m => m == UserMailings.YoungPeople) ?? false;
    }

    public IdentityDto ToDto(Guid id)
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
            Id = id,
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
