using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Boats;

[Node("Bootslager", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,boatmanager")]
public class IndexModel : PageModel
{
    private readonly Core.Repositories.Boats _boats;

    public BoatRentalListDto[] Items { get; set; } = Array.Empty<BoatRentalListDto>();

    public IndexModel(Core.Repositories.Boats boats)
    {
        _boats = boats;
    }

    public async Task OnGetAsync(int filter, CancellationToken cancellationToken)
    {
        var items = await _boats.GetRentalList(cancellationToken);
        if (filter == 0)
        {
            Items = items;    
        }
        else if (filter == 1)
        {
            Items = items.Where(i => i.Boat?.MaxRentalDays > 0).ToArray();
        }
        else
        {
            Items = items.Where(i => i.Boat?.MaxRentalDays == 0).ToArray();
        }
    }
}
