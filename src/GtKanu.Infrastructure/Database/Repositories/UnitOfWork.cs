using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.Database.Entities;

namespace GtKanu.Infrastructure.Database.Repositories;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly TimeProvider _timeProvider;
    private readonly AppDbContext _dbContext;
    private MailingRepository? _mailings;
    private EmailQueueRepository? _emailQueue;
    private MyMailingsRepository? _myMailings;
    private WikiArticleRepository? _wikiArticle;

    public IMailings Mailings =>
        _mailings ??= new(_timeProvider, _dbContext.Set<Mailing>());

    public IEmailQueues EmailQueue =>
        _emailQueue ??= new(_timeProvider, _dbContext.Set<EmailQueue>());

    public IMyMailings MyMailings =>
        _myMailings ??= new(_timeProvider, _dbContext.Set<MyMailing>());

    public IWikiArticles WikiArticles =>
        _wikiArticle ??= new(_timeProvider, _dbContext.Set<WikiArticle>());

    public UnitOfWork(
        TimeProvider timeProvider,
        AppDbContext dbContext)
    {
        _timeProvider = timeProvider;
        _dbContext = dbContext;
    }

    public Task<int> Save(CancellationToken cancellationToken) => 
        _dbContext.SaveChangesAsync(cancellationToken);
}
