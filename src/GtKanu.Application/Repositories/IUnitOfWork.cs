namespace GtKanu.Application.Repositories;

public interface IUnitOfWork
{
    IMailings Mailings { get; }
    IEmailQueues EmailQueue { get; }
    IMyMailings MyMailings { get; }
    IWikiArticles WikiArticles { get; }

    Task<int> Save(CancellationToken cancellationToken);
}
