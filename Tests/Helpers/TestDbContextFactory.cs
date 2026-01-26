using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using GameDataContext = AlienInvasionLogistics.Source.Database.Contexts.GameDataContext;

namespace AlienInvasionLogistics.Tests.Helpers;

/// <summary>
///     Factory for creating in-memory test database contexts
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    ///     Creates a new in-memory database context with a unique database name
    /// </summary>
    public static GameDataContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<GameDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new GameDataContext(options);
        return context;
    }

    /// <summary>
    ///     Creates a new in-memory database context with a specific database name (for shared context tests)
    /// </summary>
    public static GameDataContext CreateInMemoryContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<GameDataContext>()
            .UseInMemoryDatabase(databaseName)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new GameDataContext(options);
        return context;
    }
}