using System.Reflection;

namespace GtKanu.Core.Models;

public sealed class AppSettings
{
    public string Version { get; set; }
    public required string HeaderTitle { get; set; }
    public required string Slogan { get; set; }
    public required string[] InvoiceSender { get; set; }
    public string? MailingReplyTo { get; set; }
    public string? MailingFooterImageName { get; set; }

    public AppSettings()
    {
        var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.1";
        Version = version.Substring(0, Math.Min(version.Length, 16));
    }
}
