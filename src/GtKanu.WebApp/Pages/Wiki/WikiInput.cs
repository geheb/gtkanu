using GtKanu.Ui.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKanu.Ui.Pages.Wiki;

public class WikiInput
{
    [Display(Name = "Kennung")]
    [RequiredField, TextLengthField(16, MinimumLength = 1)]
    public string? Identifier { get; set; }

    [Display(Name = "Titel")]
    [RequiredField, TextLengthField]
    public string? Title { get; set; }

    [Display(Name = "Ansprechpartner")]
    public string? UserId { get; set; }

    [Display(Name = "Inhalt")]
    [RequiredField]
    public string? Content { get; set; }

    public WikiArticleDto ToDto(Guid id = default)
    {
        return new()
        {
            Id = id,
            UserId = string.IsNullOrEmpty(UserId) ? null : Guid.Parse(UserId),
            Identifier = Identifier,
            Title = Title,
            Content = Content
        };
    }

    public void FromDto(WikiArticleDto dto)
    {
        Identifier = dto.Identifier;
        Title = dto.Title;
        UserId = dto.UserId?.ToString();
        Content = dto.Content;
    }
}
