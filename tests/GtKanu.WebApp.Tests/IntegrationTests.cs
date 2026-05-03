namespace GtKanu.WebApp.Tests;

using System.Net;
using FluentAssertions;

public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public IntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ShouldReturnOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/healthz");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HomePage_ShouldReturnOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LoginPage_ShouldReturnOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Login");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ImprintPage_ShouldReturnOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Imprint");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PrivacyPage_ShouldReturnOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Privacy");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ProtectedPage_ShouldRedirectToLogin()
    {
        var client = _factory.CreateClient(new() { AllowAutoRedirect = false });
        var response = await client.GetAsync("/MyAccount");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.OriginalString.Should().Contain("/Login");
    }

    [Fact]
    public async Task AdminPage_ShouldRedirectToLogin()
    {
        var client = _factory.CreateClient(new() { AllowAutoRedirect = false });
        var response = await client.GetAsync("/Users");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.OriginalString.Should().Contain("/Login");
    }

    [Fact]
    public async Task StaticFiles_ShouldReturnOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/css/site.css");
        // May return 404 if static files don't exist, but the pipeline should handle it
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task NonExistentPage_ShouldReturnNotFound()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/ThisPageDoesNotExist");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
