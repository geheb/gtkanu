using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.MyTryouts;

[Node("Chat zum Training", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public class TryoutChatModel : PageModel
{
    private readonly Core.Repositories.Tryouts _tryouts;

    public string? TryoutDetails { get; set; }
    public TryoutChatDto[] Items { get; set; } = [];
    public bool IsDisabled { get; set; }

    [BindProperty]
    public string? Message { get; set; }

    public TryoutChatModel(Core.Repositories.Tryouts tryouts)
    {
        _tryouts = tryouts;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var tryout = await _tryouts.FindTryoutList(id, cancellationToken);
        if (tryout == null)
        {
            IsDisabled = true;
            return;
        }

        IsDisabled = tryout.IsExpired;

        var dc = new GermanDateTimeConverter();
        TryoutDetails = $"{tryout.Type} ({dc.ToDateTime(tryout.Date)}) @ {tryout.ContactName}";

        Items = await _tryouts.GetChat(id, User.GetId(), cancellationToken);
    }

    public async Task<IActionResult> OnPostMessageAsync(Guid id, string? message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message) || message.Length > 256) return new JsonResult(false);
        var result = await _tryouts.CreateChatMessage(id, User.GetId(), message, cancellationToken);
        return new JsonResult(result);
    }
}
