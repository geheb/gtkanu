using FluentResults;
using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IIdentities
{
    Task<Result> Create(IdentityDto dto, CancellationToken cancellationToken);
    Task<IdentityDto[]> GetAll(CancellationToken cancellationToken);
    Task<IdentityDto?> Find(Guid id, CancellationToken cancellationToken);
    Task<Result> Update(IdentityDto dto, CancellationToken cancellationToken);
    Task<Result> Remove(Guid id, CancellationToken cancellationToken);
    Task<Result> UpdateLoginSucceeded(Guid id, CancellationToken cancellationToken);
}
