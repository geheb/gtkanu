namespace GtKanu.Application.Tests.Models;

using GtKanu.Application.Models;

public class AppSettingsTests
{
    [Fact]
    public void Constructor_SetsDefaultVersion()
    {
        var settings = new AppSettings
        {
            HeaderTitle = "Test",
            Slogan = "Test Slogan",
            InvoiceSender = ["test@test.de"]
        };
        settings.Version.Should().NotBeNullOrEmpty();
        settings.Version.Length.Should().BeLessThanOrEqualTo(16);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var settings = new AppSettings
        {
            HeaderTitle = "GtKanu",
            Slogan = "Paddle with us",
            InvoiceSender = ["invoice@gtkanu.de"],
            MailingReplyTo = "reply@gtkanu.de",
            MailingFooterImageName = "logo.png"
        };

        settings.HeaderTitle.Should().Be("GtKanu");
        settings.Slogan.Should().Be("Paddle with us");
        settings.InvoiceSender.Should().ContainSingle().Which.Should().Be("invoice@gtkanu.de");
        settings.MailingReplyTo.Should().Be("reply@gtkanu.de");
        settings.MailingFooterImageName.Should().Be("logo.png");
    }
}
