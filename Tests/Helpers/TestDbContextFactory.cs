using Core.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using System;

namespace AlienInvasionLogistics.Tests.Helpers
{
    /// <summary>
    /// Factory for creating in-memory test database contexts
    /// </summary>
    public static class TestDbContextFactory
    {
        /// <summary>
        /// Creates a new in-memory database context with a unique database name
        /// </summary>
        public static GameDataContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<GameDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new GameDataContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        /// <summary>
        /// Creates a new in-memory database context with a specific database name (for shared context tests)
        /// </summary>
        public static GameDataContext CreateInMemoryContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<GameDataContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            var context = new GameDataContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
