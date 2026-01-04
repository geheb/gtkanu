using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.MyInvoices;

[Node("Rechnungsdetails", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public class DetailsModel : PageModel
{
    private readonly IIdentities _identityRepository;
    private readonly IInvoices _invoices;
    private readonly IBookings _bookings;

    public string? Recipient { get; set; } = "n.v.";
    public string? Description { get; set; } = "n.v.";
    public string? Period { get; set; } = "n.v.";
    public decimal Total { get; set; }
    public BookingFoodDto[] Bookings { get; set; } = [];

    public DetailsModel(
        IIdentities identityRepository,
        IInvoices invoices,
        IBookings bookings)
    {
        _identityRepository = identityRepository;
        _invoices = invoices;
        _bookings = bookings;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var invoice = await _invoices.Find(id, User.GetId(), cancellationToken);
        if (invoice == null) return;

        var user = await _identityRepository.Find(User.GetId(), cancellationToken);

        Recipient = user?.Name;
        Description = invoice.Description;
        Period = invoice.Period;
        Total = invoice.Total;

        Bookings = await _bookings.GetInvoiceBookings(id, cancellationToken);
    }
}
