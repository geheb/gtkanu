using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.MyInvoices
{
    [Node("Meine Rechnungen", FromPage = typeof(Pages.IndexModel))]
    [Authorize(Roles = "administrator,member")]
    public class IndexModel : PageModel
    {
        private readonly Core.Repositories.Invoices _invoices;
        public InvoiceDto[] Invoices { get; set; } = Array.Empty<InvoiceDto>();
        public decimal Total { get; set; }
        public decimal OpenTotal { get; set; }

        public IndexModel(Core.Repositories.Invoices invoices)
        {
            _invoices = invoices;
        }

        public async Task OnGetAsync(CancellationToken cancellationToken = default)
        {
            Invoices = await _invoices.GetAll(User.GetId(), cancellationToken);
            Total = Invoices.Sum(i => i.Total);
            OpenTotal = Invoices.Sum(i => i.OpenTotal);
        }
    }
}
