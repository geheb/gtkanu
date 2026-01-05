using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Annotations;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.WebApp.Pages.Foods;

[Node("Buchungsliste bearbeiten", FromPage = typeof(ListModel))]
[Authorize(Roles = "administrator,treasurer")]
public class EditListModel : PageModel
{
    private readonly IFoods _foods;

    [Display(Name = "Name")]
    [BindProperty, RequiredField, TextLengthField]
    public string? Name { get; set; }

    [Display(Name = "Gültig ab")]
    [BindProperty, RequiredField]
    public string? ValidFrom { get; set; }

    public bool IsDisabled { get; set; }

    public EditListModel(IFoods foods)
    {
        _foods = foods;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var foodList = await _foods.FindFoodList(id, cancellationToken);

        if (foodList == null)
        {
            IsDisabled = true;
            ModelState.AddModelError(string.Empty, "Fehler beim Laden der Buchungsliste.");
            return;
        }

        Name = foodList.Name;
        ValidFrom = new GermanDateTimeConverter().ToIso(foodList.ValidFrom);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();

        var dto = new FoodListDto();
        dto.Id = id;
        dto.Name = Name;
        dto.ValidFrom = new GermanDateTimeConverter().FromIsoDateTime(ValidFrom)!.Value;

        if (!await _foods.Update(dto, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Speichern der Buchungsliste.");
            return Page();
        }

        return RedirectToPage("Index");
    }
}
