using GameVault.Web.Models;

namespace GameVault.Web.Services
{
    public interface IGameService
    {
        Task<IEnumerable<Game>> GetAllGamesAsync();
        Task<Game?> GetGameByIdAsync(int id);
        Task<Game> AddGameAsync(Game game);
        Task<Game> UpdateGameAsync(Game game);
        Task<bool> DeleteGameAsync(int id);
        Task<IEnumerable<Game>> SearchGamesAsync(string searchTerm);
        Task<IEnumerable<Game>> GetGamesByStatusAsync(GameStatus status);
        Task<List<string>> GetGenresAsync();
        Task<List<string>> GetPlatformsAsync();
    }
}
