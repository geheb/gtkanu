using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.WebApp.Converter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Foods;

[Node("Getränke/Speisen/Spenden", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,treasurer", Policy = Policies.TwoFactorAuth)]
public class IndexModel : PageModel
{
    private readonly IBookings _bookings;

    public BookingFoodDto[] Bookings { get; set; } = [];
    public SelectListItem[] FilterItems { get; set; } = [];
    public string? FilterDate { get; set; }
    public decimal Total { get; set; }
    public decimal OpenTotal { get; set; }
    public decimal DrinksTotal { get; set; }
    public decimal DishesTotal { get; set; }
    public decimal DonationTotal { get; set; }

    public IndexModel(IBookings bookings)
    {
        _bookings = bookings;
    }

    public async Task OnGetAsync([StringLength(10)] string? filter = null, CancellationToken cancellationToken = default)
    {
        var bookingFilter = new BookingFilter();
        var filterDate = bookingFilter.ParseDateFirstOfMonth(filter);

        FilterDate = bookingFilter.ToDateFormatString(filterDate);

        FilterItems = bookingFilter.CreateListItems(filterDate);

        Bookings = await _bookings.GetUntilEndOfMonth(filterDate, cancellationToken);

        Total = Bookings.Sum(b => b.Total);
        OpenTotal = Bookings.Sum(b => b.OpenTotal);
        DrinksTotal = Bookings.Sum(b => b.Type == FoodType.Drink ? b.Total : 0);
        DishesTotal = Bookings.Sum(b => b.Type == FoodType.Dish ? b.Total : 0);
        DonationTotal = Bookings.Sum(b => b.Type == FoodType.Donation ? b.Total : 0);
    }
}
