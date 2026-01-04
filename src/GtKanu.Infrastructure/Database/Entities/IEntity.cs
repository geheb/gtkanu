namespace GtKanu.Infrastructure.Database.Entities;

public interface IEntity
{
    Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Updated { get; set; }
}
