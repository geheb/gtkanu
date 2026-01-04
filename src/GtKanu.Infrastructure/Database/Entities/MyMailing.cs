using GtKanu.Application.Converter;
using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class MyMailing : IEntity, IDtoMapper<MyMailingDto>
{
    public Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Updated { get; set; }
    public Mailing? Mailing { get; set; }
    public Guid? MailingId { get; set; }
    public IdentityUserGuid? User {  get; set; }
    public Guid? UserId { get; set; }
    public bool HasRead { get; set; }

    public void FromDto(MyMailingDto model)
    {
        Id = model.Id;
        MailingId = model.MailingId;
        UserId = model.UserId;
        HasRead = model.HasRead;
    }

    public MyMailingDto ToDto(GermanDateTimeConverter dc)
    {
        return new()
        {
            Id = Id,
            MailingId = MailingId,
            UserId = UserId,
            Created = dc.ToLocal(Created),
            ReplyAddress = Mailing!.ReplyAddress,
            Subject = Mailing!.Subject,
            HtmlBody = Mailing.HtmlBody,
            HasRead = HasRead,
        };
    }
}
