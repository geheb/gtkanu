using GtKanu.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.Ui.Pages.Foods
{
    [Node("Küchendienst", FromPage = typeof(Pages.IndexModel))]
    [Authorize(Roles = "administrator,kitchen")]
    public class KitchenServiceModel : PageModel
    {
        private readonly Core.Repositories.Bookings _bookings;

        [Display(Name = "Datum der Buchungen")]
        public string? BookingDate { get; set; }
        public bool HideCompleted { get; set; }

        public BookingFoodDto[] Bookings { get; set; } = Array.Empty<BookingFoodDto>();
        public decimal DrinksTotal { get; set; }
        public decimal DishesTotal { get; set; }
        public decimal Total { get; set; }

        public KitchenServiceModel(Core.Repositories.Bookings bookings)
        {
            _bookings = bookings;
        }

        public async Task OnGetAsync([StringLength(10)] string? filter = null, int status = 0, CancellationToken cancellationToken = default)
        {
            var bookingFilter = new BookingFilter();
            var filterDate = bookingFilter.ParseDate(filter);

            BookingDate = bookingFilter.ToDateFormatString(filterDate);
            HideCompleted = status == 1;

            Bookings = await _bookings.GetNotCancelledForOneDay(filterDate, cancellationToken);

            DrinksTotal = Bookings.Sum(b => b.Type == FoodType.Drink ? b.Total : 0);
            DishesTotal = Bookings.Sum(b => b.Type == FoodType.Dish ? b.Total : 0);
            Total = Bookings.Sum(b => b.Total);

            if (HideCompleted)
            {
                Bookings = Bookings.Where(b => b.Status == BookingStatus.Confirmed).ToArray();
            }
        }

        public async Task<IActionResult> OnPostCompletedAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _bookings.Complete(id, cancellationToken);
            return new JsonResult(result);
        }
    }
}
