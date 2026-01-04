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

[Node("Buchungsliste anlegen", FromPage = typeof(ListModel))]
[Authorize(Roles = "administrator,treasurer")]
public sealed class CreateListModel : PageModel
{
    private readonly IFoods _foods;

    [Display(Name = "Name")]
    [BindProperty, RequiredField, TextLengthField]
    public string? Name { get; set; } = $"Speisen, Getränke & Spenden {DateTime.UtcNow.Year}";

    [Display(Name = "Gültig ab")]
    [BindProperty, RequiredField]
    public string? ValidFrom { get; set; }

    public CreateListModel(IFoods foods)
    {
        _foods = foods;
        var dc = new GermanDateTimeConverter();
        ValidFrom = dc.ToIso(dc.ToLocal(DateTimeOffset.UtcNow));
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();

        var dto = new FoodListDto();
        dto.Name = Name;
        dto.ValidFrom = new GermanDateTimeConverter().FromIsoDateTime(ValidFrom)!.Value;

        if (!await _foods.Create(dto, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "Fehler beim Anlegen der Buchungsliste.");
            return Page();
        }

        return RedirectToPage("EditListItems", new { id = dto.Id });
    }
}
