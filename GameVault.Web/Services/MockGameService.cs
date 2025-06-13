using GameVault.Web.Models;

namespace GameVault.Web.Services
{
    public class MockGameService : IGameService
    {
        private List<Game> _games = new();
        private int _nextId = 1;

        public MockGameService()
        {
            InitializeMockData();
        }

        private void InitializeMockData()
        {
            _games = new List<Game>
            {
                new Game
                {
                    Id = _nextId++,
                    Title = "The Legend of Zelda: Breath of the Wild",
                    Description = "An action-adventure game set in a large open world.",
                    Genre = "Action-Adventure",
                    ReleaseDate = new DateTime(2017, 3, 3),
                    Rating = 9.7m,
                    Status = GameStatus.Completed,
                    DateAdded = DateTime.Now.AddDays(-30),
                    Platform = "Nintendo Switch",
                    CoverImageUrl = "https://via.placeholder.com/200x300?text=Zelda+BOTW",
                    Price = 59.99m,
                    IsCompleted = true,
                    CompletionDate = DateTime.Now.AddDays(-10),
                    PlaytimeHours = 120
                },
                new Game
                {
                    Id = _nextId++,
                    Title = "Cyberpunk 2077",
                    Description = "An open-world, action-adventure story set in Night City.",
                    Genre = "RPG",
                    ReleaseDate = new DateTime(2020, 12, 10),
                    Rating = 8.5m,
                    Status = GameStatus.Playing,
                    DateAdded = DateTime.Now.AddDays(-15),
                    Platform = "PC",
                    CoverImageUrl = "https://via.placeholder.com/200x300?text=Cyberpunk+2077",
                    Price = 39.99m,
                    IsCompleted = false,
                    PlaytimeHours = 45
                },
                new Game
                {
                    Id = _nextId++,
                    Title = "God of War",
                    Description = "A mythology-based action-adventure game.",
                    Genre = "Action-Adventure",
                    ReleaseDate = new DateTime(2018, 4, 20),
                    Rating = 9.5m,
                    Status = GameStatus.WantToPlay,
                    DateAdded = DateTime.Now.AddDays(-5),
                    Platform = "PlayStation 5",
                    CoverImageUrl = "https://via.placeholder.com/200x300?text=God+of+War",
                    Price = 49.99m,
                    IsCompleted = false,
                    PlaytimeHours = 0
                },
                new Game
                {
                    Id = _nextId++,
                    Title = "Minecraft",
                    Description = "A sandbox video game with infinite possibilities.",
                    Genre = "Sandbox",
                    ReleaseDate = new DateTime(2011, 11, 18),
                    Rating = 9.0m,
                    Status = GameStatus.OnHold,
                    DateAdded = DateTime.Now.AddDays(-60),
                    Platform = "PC",
                    CoverImageUrl = "https://via.placeholder.com/200x300?text=Minecraft",
                    Price = 26.95m,
                    IsCompleted = false,
                    PlaytimeHours = 200
                },
                new Game
                {
                    Id = _nextId++,
                    Title = "Elden Ring",
                    Description = "An action RPG set in the Lands Between.",
                    Genre = "Action RPG",
                    ReleaseDate = new DateTime(2022, 2, 25),
                    Rating = 9.8m,
                    Status = GameStatus.Dropped,
                    DateAdded = DateTime.Now.AddDays(-20),
                    Platform = "PC",
                    CoverImageUrl = "https://via.placeholder.com/200x300?text=Elden+Ring",
                    Price = 59.99m,
                    IsCompleted = false,
                    PlaytimeHours = 15
                }
            };
        }
        public Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            return Task.FromResult(_games.AsEnumerable());
        }

        public Task<Game?> GetGameByIdAsync(int id)
        {
            var game = _games.FirstOrDefault(g => g.Id == id);
            return Task.FromResult(game);
        }

        public Task<Game> AddGameAsync(Game game)
        {
            game.Id = _nextId++;
            game.DateAdded = DateTime.Now;
            _games.Add(game);
            return Task.FromResult(game);
        }
        public Task<Game> UpdateGameAsync(Game game)
        {
            var existingGame = _games.FirstOrDefault(g => g.Id == game.Id);
            if (existingGame != null)
            {
                var index = _games.IndexOf(existingGame);
                _games[index] = game;
                return Task.FromResult(game);
            }
            return Task.FromResult(game); // Return the game even if not found for consistency
        }

        public Task<bool> DeleteGameAsync(int id)
        {
            var game = _games.FirstOrDefault(g => g.Id == id);
            if (game != null)
            {
                _games.Remove(game);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        public Task<IEnumerable<Game>> GetGamesByStatusAsync(GameStatus status)
        {
            var filteredGames = _games.Where(g => g.Status == status);
            return Task.FromResult(filteredGames);
        }

        public Task<IEnumerable<Game>> SearchGamesAsync(string searchTerm)
        {
            var filteredGames = _games.Where(g =>
                g.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                g.Genre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                g.Developer.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                g.Publisher.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(filteredGames);
        }

        public Task<List<string>> GetGenresAsync()
        {
            var genres = _games.Select(g => g.Genre).Where(g => !string.IsNullOrEmpty(g)).Distinct().OrderBy(g => g).ToList();
            return Task.FromResult(genres);
        }

        public Task<List<string>> GetPlatformsAsync()
        {
            var platforms = _games.Select(g => g.Platform).Where(p => !string.IsNullOrEmpty(p)).Distinct().OrderBy(p => p).ToList();
            return Task.FromResult(platforms);
        }
    }
}
