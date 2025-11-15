using Core.Controllers.UI.NewGameScreen;
using Core.Database.Models;
using Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Database
{
    public interface IGameDataService
    {
        Task CreateNewGameAsync(GameSettings settings);
        Task<List<GameSaveModel>> GetAllSavesAsync();
        Task<GameDataModel> LoadGameAsync(string saveId);
        Task SaveGameAsync(GameSaveData gameSave);
        Task ClearDatabaseAsync();
    }
}
