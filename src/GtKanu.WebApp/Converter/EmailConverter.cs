namespace GtKanu.Ui.Converter;

public sealed class EmailConverter
{
    public string Anonymize(string email)
    {
        var emailSplit = email.Split('@');
        var emailUser = emailSplit[0];

        var emailDomain = emailSplit[1];
        return emailUser[0] + "***@" + emailDomain;
    }
}
