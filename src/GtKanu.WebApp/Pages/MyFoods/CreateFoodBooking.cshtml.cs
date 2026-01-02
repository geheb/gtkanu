using GtKanu.Core.User;
using GtKanu.Ui.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.Ui.Pages.MyFoods;

[Node("Getränk/Speise/Spende buchen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public class CreateFoodBookingModel : PageModel
{
    private readonly Core.Repositories.IdentityRepository _identityRepository;
    private readonly Core.Repositories.Foods _foods;
    private readonly Core.Repositories.Bookings _bookings;

    [Display(Name = "Getränk/Speise/Spende")]
    [BindProperty, RequiredField]
    public Guid SelectedFoodId { get; set; }

    public const int MinCountFood = 1;
    public const int MaxCountFood = 10;
    public const int MaxCountFoodLen = 2;

    [Display(Name = "Anzahl")]
    [BindProperty, RequiredField]
    [RangeField(MinCountFood, MaxCountFood)]
    public int CountFood { get; set; } = MinCountFood;
    
    public FoodDto[] Foods { set; get; } = Array.Empty<FoodDto>();
    public bool IsDisabled { get; set; }

    public CreateFoodBookingModel(
        Core.Repositories.IdentityRepository identityRepository,
        Core.Repositories.Foods foods, 
        Core.Repositories.Bookings bookings)
    {
        _identityRepository = identityRepository;
        _foods = foods;
        _bookings = bookings;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Foods = await _foods.GetLatestFoods(cancellationToken);
        if (Foods.Length < 1)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Keine gültige Buchungsliste vorhanden.");
            return;
        }

        var user = await _identityRepository.Find(User.GetId(), cancellationToken);
        IsDisabled = user is null || !user.Value.IsEnabledForBookings;
        if (IsDisabled)
        {
            ModelState.AddModelError(string.Empty, "Aufgrund fehlender Daten für die Rechnungsstellung ist eine Buchung nicht möglich.");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            Foods = await _foods.GetLatestFoods(cancellationToken);
            return Page();
        }

        var result = await _bookings.Create(User.GetId(), SelectedFoodId, CountFood, cancellationToken);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Speichern der Buchung.");
            return Page();
        }

        return RedirectToPage("Index");
    }
}
