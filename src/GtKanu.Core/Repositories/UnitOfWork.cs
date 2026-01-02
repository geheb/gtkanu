using GtKanu.Core.Database;
using GtKanu.Core.Entities;

namespace GtKanu.Core.Repositories;

public sealed class UnitOfWork
{
    private readonly TimeProvider _timeProvider;
    private readonly AppDbContext _dbContext;
    private MailingRepository? _mailings;
    private EmailQueueRepository? _emailQueue;
    private MyMailingsRepository? _myMailings;
    private WikiArticleRepository? _wikiArticle;

    public MailingRepository Mailings =>
        _mailings ??= new(_timeProvider, _dbContext.Set<Mailing>());

    public EmailQueueRepository EmailQueue =>
        _emailQueue ??= new(_timeProvider, _dbContext.Set<EmailQueue>());

    public MyMailingsRepository MyMailings =>
        _myMailings ??= new(_timeProvider, _dbContext.Set<MyMailing>());

    public WikiArticleRepository WikiArticles =>
        _wikiArticle ??= new(_timeProvider, _dbContext.Set<WikiArticle>());

    public UnitOfWork(
        TimeProvider timeProvider,
        AppDbContext dbContext)
    {
        _timeProvider = timeProvider;
        _dbContext = dbContext;
    }

    public Task<int> Save(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
