using FluentResults;
using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IMyMailings
{
    void Create(MyMailingDto model);
    Task<MyMailingDto?> Find(Guid id, CancellationToken cancellationToken);
    Task<MyMailingDto[]> GetByUser(Guid userId, CancellationToken cancellationToken);
    Task<Result> UpdateHasRead(Guid id, CancellationToken cancellationToken);
}
