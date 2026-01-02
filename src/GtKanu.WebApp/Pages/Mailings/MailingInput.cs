using GtKanu.Core.Email;
using GtKanu.Ui.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.Ui.Pages.Mailings;

public sealed class MailingInput
{
    [Display(Name = "Antwort-Adresse")]
    [EmailField, EmailLengthField]
    public string? ReplyAddress { get; set; }

    [Display(Name = "Nur Mitglieder")]
    public bool IsMemberOnly { get; set; }

    [Display(Name = "Jugend")]
    public bool IsYoungPeople { get; set; }

    [Display(Name = "Individuelle E-Mail-Adressen (Komma getrennt)")]
    [TextLengthField(MinimumLength = 6)]
    public string? OtherRecipients { get; set; }

    [Display(Name = "Betreff")]
    [RequiredField, TextLengthField]
    public string? Subject { get; set; }

    [Display(Name = "Inhalt")]
    [RequiredField]
    public string? Body { get; set; }

    internal void From(MailingDto dto)
    {
        ReplyAddress = dto.ReplyAddress;
        IsMemberOnly = dto.IsMemberOnly;
        IsYoungPeople = dto.IsYoungPeople;
        OtherRecipients = dto.OtherRecipients is not null ? string.Join(',', dto.OtherRecipients) : null;
        Subject = dto.Subject;
        Body = dto.Body;
    }

    internal MailingDto ToDto(Guid id = default) => new()
    {
        Id = id,
        ReplyAddress = ReplyAddress,
        IsMemberOnly = IsMemberOnly,
        IsYoungPeople = IsYoungPeople,
        OtherRecipients = OtherRecipients?.Split(',', StringSplitOptions.RemoveEmptyEntries),
        Subject = Subject,
        Body = Body
    };

    internal async Task<string[]> Validate(EmailValidatorService emailValidator, CancellationToken cancellationToken)
    {
        var result = new List<string>();

        var emailAttr = new EmailAddressAttribute();

        if (!string.IsNullOrWhiteSpace(ReplyAddress))
        {
            if (!emailAttr.IsValid(ReplyAddress))
            {
                result.Add($"Die Antwort-Adresse {ReplyAddress} ist fehlerhaft.");
            }

            if (!await emailValidator.Validate(ReplyAddress, cancellationToken))
            {
                result.Add($"Die Antwort-Adresse {ReplyAddress} ist ungültig.");
            }
        }

        if (string.IsNullOrWhiteSpace(OtherRecipients) && !IsMemberOnly && !IsYoungPeople)
        {
            result.Add("Keine Empfänger angegeben.");
        }

        if (!string.IsNullOrWhiteSpace(OtherRecipients))
        {
            foreach (var recipient in OtherRecipients.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!emailAttr.IsValid(recipient))
                {
                    result.Add($"Die Empfänger E-Mail-Adresse {recipient} ist fehlerhaft.");
                    continue;
                }

                if (!await emailValidator.Validate(recipient, cancellationToken))
                {
                    result.Add($"Die Empfänger E-Mail-Adresse {recipient} ist ungültig.");
                }
            }
        }

        return [.. result];
    }
}
