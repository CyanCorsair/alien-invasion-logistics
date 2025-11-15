using Microsoft.EntityFrameworkCore;
using Core.Database.Contexts;

namespace Core.Database
{
    /// <summary>
    /// Factory for creating GameDataContext instances with proper lifecycle management.
    /// Non-static to support dependency injection and hold configured DbContextOptions.
    /// </summary>
    public interface IGameDataContextFactory
    {
        GameDataContext CreateDbContext();
    }

    public class GameDataContextFactory : IGameDataContextFactory
    {
        private readonly DbContextOptions<GameDataContext> _options;

        public GameDataContextFactory(DbContextOptions<GameDataContext> options)
        {
            _options = options;
        }

        public GameDataContext CreateDbContext()
        {
            return new GameDataContext(_options);
        }
    }
}
