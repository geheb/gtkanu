using FluentResults;
using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IWikiArticles
{
    void Create(WikiArticleDto model);
    Task<Result> Delete(Guid id, CancellationToken cancellationToken);
    Task<WikiArticleDto?> Find(Guid id, CancellationToken cancellationToken);
    Task<bool> FindIdentifier(string identifier, CancellationToken cancellationToken);
    Task<WikiArticleDto[]> GetAll(CancellationToken cancellationToken);
    Task<Result> Update(WikiArticleDto model, CancellationToken cancellationToken);
}
