namespace GtKanu.Application.Models;

using System;

public struct WikiArticleDto : IDto
{
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public string? Title { get; set; }
    public Guid? UserId { get; set; }
    public string? User { get; set; }
    public string? UserEmail { get; set; }
    public DateTimeOffset LastUpdate { get; set; }
    public string? Content { get; set; }
}
