namespace GtKanu.Core.Entities;

internal sealed class TryoutChat
{
    public Guid Id { get; set; }
    public Guid? TryoutId { get; set; }
    public Tryout? Tryout { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? Message { get; set; }
}
