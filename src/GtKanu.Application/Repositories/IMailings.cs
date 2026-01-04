using FluentResults;
using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IMailings
{
    void Create(MailingDto model);
    Task<MailingDto?> Find(Guid id, CancellationToken cancellationToken);
    Task<MailingDto[]> GetAll(CancellationToken cancellationToken);
    Task<Result> Update(MailingDto model, CancellationToken cancellationToken);
    Task<Result> UpdateClosed(Guid id, int emailCount, CancellationToken cancellationToken);
}
