namespace GtKanu.WebApp.Tests;

using System.Net;

[Collection("WebApp")]
public class ErrorPageIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;

    public ErrorPageIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task NonExistentPage_ShouldReturnNotFound()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/ThisPageDoesNotExistAtAll");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Error403_ShouldReturnForbiddenPage()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/Error/403");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Error404_ShouldReturnNotFoundPage()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/Error/404");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Error500_ShouldReturnErrorPage()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/Error/500");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
