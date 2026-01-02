namespace GtKanu.Core.Entities;

using GtKanu.Core.Converter;
using GtKanu.Core.Models;
using System;

public sealed class WikiArticle : IEntity, IDtoMapper<WikiArticleDto>
{
    public Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Updated { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public string? Identifier { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }

    public void FromDto(WikiArticleDto model)
    {
        Id = model.Id;
        UserId = model.UserId;
        Identifier = model.Identifier;
        Title = model.Title;
        Content = model.Content;
    }

    public WikiArticleDto ToDto(GermanDateTimeConverter dc)
    {
        return new()
        {
            Id = Id,
            LastUpdate = dc.ToLocal(Updated ?? Created),
            UserId = UserId,
            User = User?.Name,
            UserEmail = User?.EmailConfirmed == true ? User.Email : null,
            Identifier = Identifier,
            Title = Title,
            Content = Content
        };
    }
}
