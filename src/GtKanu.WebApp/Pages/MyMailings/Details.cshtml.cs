using GtKanu.Core.Repositories;
using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.MyMailings;

[Node("Mailing", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public sealed class DetailsModel : PageModel
{
    private UnitOfWork _unitOfWork;

    public MyMailingDto? Item { get; set; }

    public DetailsModel(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.MyMailings.Find(id, cancellationToken);
        if (item?.UserId == User.GetId())
        {
            await _unitOfWork.MyMailings.UpdateHasRead(id, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            Item = item;
        }
    }
}
