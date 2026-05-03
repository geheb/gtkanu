namespace GtKanu.Infrastructure.Tests.Database;

using System.Data.Common;
using GtKanu.Infrastructure.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

internal sealed class TestDbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;
    private bool _initialized;

    public TestDbContextFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:;Cache=Shared");
        _connection.Open();
    }

    public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new AppDbContext(options);

        if (!_initialized)
        {
            context.Database.EnsureCreated();
            _initialized = true;
        }

        return context;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
