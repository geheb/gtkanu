namespace GtKanu.WebApp.Tests;

using System.Net;
using GtKanu.Application.Models;

[Collection("WebApp")]
public class SmokeTests
{
    private readonly CustomWebApplicationFactory _factory;

    public SmokeTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Factory_CreateClient_ShouldWork()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Factory_CreateAuthenticatedClient_ShouldWork()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task EnsureUser_ShouldNotBreakFactory()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("smoke@example.com", "Smoke1234!", Roles.Member);
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/Login");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LoginMember_ShouldAccessMyAccount()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("member-smoke@example.com", "Member1234!", Roles.Member);
        var client = _factory.CreateAuthenticatedClient();
        var loginOk = await _factory.LoginAsync(client, "member-smoke@example.com", "Member1234!");
        loginOk.Should().BeTrue();

        var myAccount = await client.GetAsync("/MyAccount");
        myAccount.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LoginAdmin_ShouldAccessMyAccount()
    {
        await _factory.EnsureAppStartedAndTestUserCreated("admin-smoke@example.com", "Admin1234!", Roles.Admin);
        var client = _factory.CreateAuthenticatedClient();
        var loginOk = await _factory.LoginAsync(client, "admin-smoke@example.com", "Admin1234!");
        loginOk.Should().BeTrue();

        var myAccount = await client.GetAsync("/MyAccount");
        myAccount.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
