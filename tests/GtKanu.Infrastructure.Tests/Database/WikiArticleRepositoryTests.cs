namespace GtKanu.Infrastructure.Tests.Database;

using GtKanu.Application.Models;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Database.Repositories;

public class WikiArticleRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Create_ShouldAddArticle()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new WikiArticleRepository(timeProvider, context.WikiArticles);

        var dto = new WikiArticleDto
        {
            Id = Guid.NewGuid(),
            Identifier = "test",
            Title = "Test Article",
            Content = "Content"
        };

        repo.Create(dto);
        await context.SaveChangesAsync();

        var all = await repo.GetAll(CancellationToken.None);
        all.Should().ContainSingle();
        all[0].Title.Should().Be("Test Article");
    }

    [Fact]
    public async Task Get_ShouldReturnArticlesByIds()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new WikiArticleRepository(timeProvider, context.WikiArticles);

        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        context.WikiArticles.Add(new WikiArticle { Id = id1, Created = timeProvider.GetUtcNow(), Identifier = "a1", Title = "A1", Content = "C1" });
        context.WikiArticles.Add(new WikiArticle { Id = id2, Created = timeProvider.GetUtcNow(), Identifier = "a2", Title = "A2", Content = "C2" });
        await context.SaveChangesAsync();

        var result = await repo.Get([id1, id2], CancellationToken.None);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Update_ShouldModifyArticle()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new WikiArticleRepository(timeProvider, context.WikiArticles);

        var id = Guid.NewGuid();
        context.WikiArticles.Add(new WikiArticle { Id = id, Created = timeProvider.GetUtcNow(), Identifier = "upd", Title = "Old", Content = "C" });
        await context.SaveChangesAsync();

        var result = await repo.Update(new WikiArticleDto { Id = id, Identifier = "upd", Title = "New", Content = "C" }, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        await context.SaveChangesAsync();

        var found = await repo.Find(id, CancellationToken.None);
        found!.Value.Title.Should().Be("New");
    }
}
