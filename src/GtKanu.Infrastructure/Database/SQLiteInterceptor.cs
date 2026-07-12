using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace GtKanu.Infrastructure.Database;

internal sealed class SQLiteInterceptor : DbConnectionInterceptor
{
    private static readonly string _pragmas = $"""
        PRAGMA synchronous=NORMAL;
        PRAGMA busy_timeout = 5000;
        PRAGMA cache_size=-65536;
        PRAGMA mmap_size=1073741824;
        PRAGMA secure_delete=1;
        PRAGMA temp_store=MEMORY;
    """;

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = _pragmas;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        using var command = connection.CreateCommand();
        command.CommandText = _pragmas;
        command.ExecuteNonQuery();
    }
}
