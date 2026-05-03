namespace GtKanu.WebApp.Tests;

using System.Net;

[Collection("WebApp")]
public class PublicPagesIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;

    public PublicPagesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ShouldReturnOk()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/healthz");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HomePage_ShouldReturnOk()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LoginPage_ShouldReturnOk()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/Login");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ImprintPage_ShouldReturnOk()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/Imprint");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PrivacyPage_ShouldReturnOk()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/Privacy");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PasswordForgottenPage_ShouldReturnOk()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/Login/PasswordForgotten");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PublicTripsPage_ShouldReturnOk()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/Trips/Public");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task StaticFiles_Css_ShouldReturnOkOrNotFound()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/css/site.css");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }
}
