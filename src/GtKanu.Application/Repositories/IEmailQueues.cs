using FluentResults;
using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IEmailQueues
{
    void Create(EmailQueueDto model);
    Task<EmailQueueDto[]> GetNextToSend(int count, CancellationToken cancellationToken);
    Task<Result> UpdateSent(Guid[] ids, CancellationToken cancellationToken);
    Task<Result> UpdateNextSchedule(Guid id, string lastError, CancellationToken cancellationToken);
}
