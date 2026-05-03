namespace GtKanu.WebApp.Tests;

using System.Net;
using GtKanu.Application.Models;

[Collection("WebApp")]
public class AdminIntegrationTests
{
    private readonly CustomWebApplicationFactory _factory;

    public AdminIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> GetAdminClientAsync()
    {
        const string password = "Admin$%1234!";
        await _factory.EnsureAppStartedAndTestUserCreated("admin@example.com", password, Roles.Admin);
        var client = _factory.CreateAuthenticatedClient();
        await _factory.LoginAsync(client, "admin@example.com", password);
        return client;
    }

    private async Task<HttpClient> GetMemberClientAsync()
    {
        const string password = "Member$%1234!";
        await _factory.EnsureAppStartedAndTestUserCreated("member@example.com", password, Roles.Member);
        var client = _factory.CreateAuthenticatedClient();
        await _factory.LoginAsync(client, "plainmember@example.com", password);
        return client;
    }

    [Fact]
    public async Task UsersPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UsersPage_Member_ShouldRedirectOrBeForbidden()
    {
        var client = await GetMemberClientAsync();
        var response = await client.GetAsync("/Users");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task BoatsPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Boats");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task FleetPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Fleet");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task FoodsPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Foods");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task FoodsCreateListPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Foods/CreateList");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MailingsPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Mailings");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MailingsCreatePage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Mailings/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task TripsPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Trips");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task TripsCreateTripPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Trips/CreateTrip");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task TryoutsPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Tryouts");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ClubhousePage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Clubhouse");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ClubhouseCreateBookingPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Clubhouse/CreateBooking");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task InvoicesPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Invoices");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WikiPage_Admin_ShouldReturnOk()
    {
        var client = await GetAdminClientAsync();
        var response = await client.GetAsync("/Wiki");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminPage_WithoutAuth_ShouldRedirectToLogin()
    {
        var client = _factory.CreateAuthenticatedClient();
        var response = await client.GetAsync("/Users");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }
}
