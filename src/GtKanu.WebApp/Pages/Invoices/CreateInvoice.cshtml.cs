using GtKanu.Ui.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace GtKanu.Ui.Pages.Invoices
{
    [Node("Rechnungen anlegen", FromPage = typeof(IndexModel))]
    [Authorize(Roles = "administrator,treasurer")]
    public class CreateInvoiceModel : PageModel
    {
        private static readonly DateTime _fixedStartDate = new DateTime(2022, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        private readonly Core.Repositories.Bookings _bookings;
        private readonly Core.Repositories.Invoices _invoices;

        [Display(Name = "Beschreibung")]
        [BindProperty, RequiredField, TextLengthField]
        public string? Description { get; set; } = "Rechnung";

        [Display(Name = "Summe")]
        public string? CurrentTotal { get; set; }

        [Display(Name = "Buchungen ab")]
        public SelectListItem[] BookingDates { get; set; } = Array.Empty<SelectListItem>();

        [Display(Name = "Buchungen bis")]
        public string? BookingDateTo { get; set; }

        public CreateInvoiceModel(
            Core.Repositories.Bookings bookings,
            Core.Repositories.Invoices invoices)
        {
            _bookings = bookings;
            _invoices = invoices;
        }

        public async Task OnGetAsync([StringLength(41)] string? filter = null, CancellationToken cancellationToken = default)
        {
            await UpdateDetails(filter, cancellationToken);
        }

        public async Task<IActionResult> OnPostAsync([StringLength(41)] string? filter = null, CancellationToken cancellationToken = default)
        {
            var filterDate = await UpdateDetails(filter, cancellationToken);

            if (!ModelState.IsValid) return Page();

            var count = await _invoices.Create(filterDate.start, filterDate.end, Description!, cancellationToken);
            if (count < 1)
            {
                ModelState.AddModelError(string.Empty, count < 0 ? "Fehler beim Anlegen der Rechnungen." : "Keine gültigen Buchungen gefunden.");
                return Page();
            }
            
            return RedirectToPage("Index");
        }

        private async Task<(DateTime start, DateTime end)> UpdateDetails(string? filter, CancellationToken cancellationToken)
        {
            var ci = CultureInfo.GetCultureInfo("de-DE");

            BookingDates = CreateBookingDateList(ci);
            var selectedItem = BookingDates.FirstOrDefault(s => s.Value.Equals(filter));
            if (selectedItem == null) selectedItem = BookingDates[0];
            selectedItem.Selected = true;

            var filterDates = selectedItem.Value.Split(';');

            var start = DateTime.FromBinary(long.Parse(filterDates[0]));
            var end = DateTime.FromBinary(long.Parse(filterDates[1]));

            var total = await _bookings.CalcTotal(start, end, cancellationToken);
            CurrentTotal = $"{total:0.00} €";

            BookingDateTo = end.ToString("g", ci);

            return (start, end);
        }

        private SelectListItem[] CreateBookingDateList(IFormatProvider formatProvider)
        {
            var items = new List<SelectListItem>();

            var start = DateTime.UtcNow.AddDays(-DateTime.UtcNow.Day + 1).AddMonths(-1).Date;
            var daysInMonth = DateTime.DaysInMonth(start.Year, start.Month);
            var end = start.AddDays(daysInMonth - start.Day)
                .Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

            var endFormatted = end.ToBinary().ToString();

            for (int i = 0; i < 12; i++)
            {
                var name = start.ToString("d", formatProvider);
                var startFormatted = start.ToBinary().ToString();
                items.Add(new SelectListItem(name, startFormatted + ";" + endFormatted));

                if (start <= _fixedStartDate) break;

                start = start.AddMonths(-1);
            }

            return items.ToArray();
        }
    }
}
