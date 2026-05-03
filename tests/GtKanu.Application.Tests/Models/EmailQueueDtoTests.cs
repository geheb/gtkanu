namespace GtKanu.Application.Tests.Models;

using FluentAssertions;
using GtKanu.Application.Models;

public class EmailQueueDtoTests
{
    [Fact]
    public void ToEmailItem_ShouldMapProperties()
    {
        var dto = new EmailQueueDto
        {
            Recipient = "user@test.de",
            Subject = "Test Subject",
            HtmlBody = "<p>Hello</p>",
            ReplyAddress = "reply@test.de"
        };

        var item = dto.ToEmailItem();
        item.Recipient.Should().Be("user@test.de");
        item.Subject.Should().Be("Test Subject");
        item.HtmlBody.Should().Be("<p>Hello</p>");
        item.ReplyAddress.Should().Be("reply@test.de");
    }
}
