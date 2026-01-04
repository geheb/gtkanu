namespace GtKanu.Application.Models;

public sealed record UserTwoFactor(bool IsEnabled, string SecretKey, string AuthUri);
