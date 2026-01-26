using System.Collections.Generic;
using System.Threading.Tasks;
using AlienInvasionLogistics.Source.Database.Dtos;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Types;
using AlienInvasionLogistics.Source.UI.MenuControllers;

namespace AlienInvasionLogistics.Source.Database;

public interface IGameDataService
{
    Task CreateNewGameAsync(GameSettings settings);
    Task<List<GameSession>> GetAllSavesAsync();
    Task<GameSession> LoadGameAsync(string saveId);
    Task SaveGameAsync(GameSession gameSession);
    Task ClearDatabaseAsync();
}