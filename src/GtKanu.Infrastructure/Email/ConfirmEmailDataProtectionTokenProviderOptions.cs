using Microsoft.AspNetCore.Identity;

namespace GtKanu.Infrastructure.Email;

public sealed class ConfirmEmailDataProtectionTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public static readonly string ProviderName = nameof(ConfirmEmailDataProtectionTokenProviderOptions);

    public ConfirmEmailDataProtectionTokenProviderOptions()
    {
        Name = ProviderName;
        TokenLifespan = TimeSpan.FromDays(3);
    }
}
