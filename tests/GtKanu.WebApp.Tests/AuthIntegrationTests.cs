namespace GtKanu.WebApp.Tests;

using System.Net;
using GtKanu.Application.Models;

[Collection("WebApp")]
public class AuthIntegrationTests
{
    private const string _userPassword = "Test$§1234!";
    private readonly CustomWebApplicationFactory _factory;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task LoginPage_Get_ShouldReturnOk()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/Login");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldStayOnLoginPage()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("test@example.com", _userPassword, Roles.Member);
        var client = _factory.CreateAuthenticatedClient();

        var loginPage = await client.GetAsync("/Login");
        var html = await loginPage.Content.ReadAsStringAsync();
        var token = _factory.ExtractAntiForgeryToken(html);

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Email"] = "test@example.com",
            ["Password"] = "wrongpassword",
            ["__RequestVerificationToken"] = token
        });

        var response = await client.PostAsync("/Login", content);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldRedirectToHome()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("member1@example.com", _userPassword, Roles.Member);
        var client = _factory.CreateAuthenticatedClient();

        var result = await _factory.LoginAsync(client, "member1@example.com", _userPassword);
        result.Should().BeTrue();

        var home = await client.GetAsync("/MyAccount");
        home.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MyAccount_WithoutAuth_ShouldRedirectToLogin()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/MyAccount");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.OriginalString.Should().Contain("/Login");
    }

    [Fact]
    public async Task MyAccount_WithAuth_ShouldReturnOk()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("member2@example.com", _userPassword, Roles.Member);
        var client = _factory.CreateAuthenticatedClient();
        await _factory.LoginAsync(client, "member2@example.com", _userPassword);

        var response = await client.GetAsync("/MyAccount");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_ShouldSignOutAndRedirect()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("member3@example.com", _userPassword, Roles.Member);
        var client = _factory.CreateAuthenticatedClient();
        await _factory.LoginAsync(client, "member3@example.com", _userPassword);

        var before = await client.GetAsync("/MyAccount");
        before.StatusCode.Should().Be(HttpStatusCode.OK);

        await _factory.LogoutAsync(client);

        var after = await client.GetAsync("/MyAccount");
        after.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task ChangePassword_WithoutAuth_ShouldRedirectToLogin()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/MyAccount/ChangePassword");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task ChangePassword_WithAuth_ShouldReturnOk()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("member4@example.com", _userPassword, Roles.Member);
        var client = _factory.CreateAuthenticatedClient();
        await _factory.LoginAsync(client, "member4@example.com", _userPassword);

        var response = await client.GetAsync("/MyAccount/ChangePassword");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MyBoats_WithoutAuth_ShouldRedirect()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/MyBoats");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task MyBoats_WithAuth_ShouldReturnOk()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("member5@example.com", _userPassword, Roles.Member);
        var client = _factory.CreateAuthenticatedClient();
        await _factory.LoginAsync(client, "member5@example.com", _userPassword);

        var response = await client.GetAsync("/MyBoats");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MyTrips_WithoutAuth_ShouldRedirect()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/MyTrips");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task MyTrips_WithAuth_ShouldReturnOk()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("member6@example.com", _userPassword, Roles.Member);
        var client = _factory.CreateAuthenticatedClient();
        await _factory.LoginAsync(client, "member6@example.com", _userPassword);

        var response = await client.GetAsync("/MyTrips");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MyFleet_WithoutAuth_ShouldRedirect()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/MyFleet");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task MyFleet_WithAuth_ShouldReturnOk()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("member7@example.com", _userPassword, Roles.Member);
        var client = _factory.CreateAuthenticatedClient();
        await _factory.LoginAsync(client, "member7@example.com", _userPassword);

        var response = await client.GetAsync("/MyFleet");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MyMailings_WithoutAuth_ShouldRedirect()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/MyMailings");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task MyMailings_WithAuth_ShouldReturnOk()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("member8@example.com", _userPassword, Roles.Member);
        var client = _factory.CreateAuthenticatedClient();
        await _factory.LoginAsync(client, "member8@example.com", _userPassword);

        var response = await client.GetAsync("/MyMailings");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MyInvoices_WithoutAuth_ShouldRedirect()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/MyInvoices");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task MyInvoices_WithAuth_ShouldReturnOk()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("member9@example.com", _userPassword, Roles.Member);
        var client = _factory.CreateAuthenticatedClient();
        await _factory.LoginAsync(client, "member9@example.com", _userPassword);

        var response = await client.GetAsync("/MyInvoices");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
