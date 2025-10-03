using GameVault.Web.Models;
using GameVault.Web.DTOs;
using System.Text.Json;
using System.Text;

namespace GameVault.Web.Services
{
    public class GameApiService : IGameService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GameApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public GameApiService(HttpClient httpClient, ILogger<GameApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/UserGames");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var dtoList = JsonSerializer.Deserialize<List<UserGameStatusResponseDto>>(content, _jsonOptions);

                return dtoList?.Select(MapFromDto) ?? new List<Game>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching games from API");
                return new List<Game>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching games");
                return new List<Game>();
            }
        }
        public async Task<Game?> GetGameByIdAsync(int id)
        {
            try
            {
                // Since the backend uses Guid IDs, we need to get all games and find by our frontend ID
                // This is not ideal for performance but works for the integration
                var allGames = await GetAllGamesAsync();
                return allGames.FirstOrDefault(g => g.Id == id);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching game {GameId} from API", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching game {GameId}", id);
                return null;
            }
        }

        public async Task<Game> AddGameAsync(Game game)
        {
            try
            {
                var request = MapToAddRequest(game);
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/UserGames", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<UserGameStatusResponseDto>(responseContent, _jsonOptions);

                return dto != null ? MapFromDto(dto) : game;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error adding game to API");
                throw new InvalidOperationException("Failed to add game", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding game");
                throw;
            }
        }

        public async Task<Game> UpdateGameAsync(Game game)
        {
            try
            {
                var request = new UpdateGameStatusRequestDto
                {
                    NewStatus = MapToBackendStatus(game.Status)
                };

                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/UserGames/{game.UserGameId}", content);
                response.EnsureSuccessStatusCode();

                return game;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error updating game {GameId} via API", game.Id);
                throw new InvalidOperationException("Failed to update game", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating game {GameId}", game.Id);
                throw;
            }
        }
        public async Task<bool> DeleteGameAsync(int id)
        {
            try
            {
                // For delete operations, we need to use the UserGameId (Guid) not the frontend ID
                // This is a limitation - we'll need to get the game first to get its UserGameId
                var game = await GetGameByIdAsync(id);
                if (game == null) return false;

                var response = await _httpClient.DeleteAsync($"api/UserGames/{game.UserGameId}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error deleting game {GameId} via API", id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting game {GameId}", id);
                return false;
            }
        }

        public async Task<IEnumerable<Game>> SearchGamesAsync(string searchTerm)
        {
            // For now, get all games and filter client-side
            // In a real application, you'd want server-side filtering
            var allGames = await GetAllGamesAsync();
            return allGames.Where(g =>
                g.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                g.Genre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                g.Developer.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                g.Publisher.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Game>> GetGamesByStatusAsync(Models.GameStatus status)
        {
            var allGames = await GetAllGamesAsync();
            return allGames.Where(g => g.Status == status);
        }
        public async Task<List<string>> GetGenresAsync()
        {
            var allGames = await GetAllGamesAsync();
            return allGames.Select(g => g.Genre).Where(g => !string.IsNullOrEmpty(g)).Distinct().OrderBy(g => g).ToList();
        }

        public async Task<List<string>> GetPlatformsAsync()
        {
            var allGames = await GetAllGamesAsync();
            return allGames.Select(g => g.Platform).Where(p => !string.IsNullOrEmpty(p)).Distinct().OrderBy(p => p).ToList();
        }

        // Mapping methods
        private Game MapFromDto(UserGameStatusResponseDto dto)
        {
            return new Game
            {
                Id = dto.Id.GetHashCode(), // Temporary ID mapping - you might want to handle this differently
                UserGameId = dto.Id,
                Title = dto.GameName,
                GameId = dto.GameId,
                Description = dto.GameDescription ?? "",
                Genre = dto.GameGenre ?? "",
                ReleaseDate = dto.GameReleaseDate ?? DateTime.MinValue,
                Rating = dto.PersonalRating ?? 0,
                Status = MapFromBackendStatus(dto.Status),
                DateAdded = dto.DateAdded,
                Platform = string.Join(", ", dto.GamePlatforms ?? Array.Empty<string>()),
                CoverImageUrl = dto.GameCoverImageUrl ?? "",
                Price = 0, // Not available in DTO
                IsCompleted = dto.Status == BackendGameStatus.Completed,
                CompletionDate = dto.Status == BackendGameStatus.Completed ? dto.DateModified : null,
                PlaytimeHours = 0, // Not available in DTO
                Notes = dto.Notes ?? "",
                Developer = dto.GameDeveloper ?? "",
                Publisher = dto.GamePublisher ?? "",
                TrailerUrl = dto.GameTrailerUrl ?? ""
            };
        }

        private AddGameToCollectionRequestDto MapToAddRequest(Game game)
        {
            return new AddGameToCollectionRequestDto
            {
                GameId = game.GameId,
                InitialStatus = MapToBackendStatus(game.Status),
                PersonalRating = game.Rating > 0 ? (int)game.Rating : null,
                Notes = game.Notes,
                game = new BackendGame
                {
                    Id = game.GameId,
                    Name = game.Title,
                    Description = game.Description,
                    Developer = game.Developer,
                    Publisher = game.Publisher,
                    ReleaseDate = game.ReleaseDate == DateTime.MinValue ? null : game.ReleaseDate,
                    Genre = game.Genre,
                    CoverImageUrl = game.CoverImageUrl,
                    TrailerUrl = game.TrailerUrl,
                    Platforms = string.IsNullOrEmpty(game.Platform) ? Array.Empty<string>() : game.Platform.Split(", ")
                }
            };
        }

        private Models.GameStatus MapFromBackendStatus(BackendGameStatus backendStatus)
        {
            return backendStatus switch
            {
                BackendGameStatus.Owned => Models.GameStatus.WantToPlay,
                BackendGameStatus.Playing => Models.GameStatus.Playing,
                BackendGameStatus.BackLog => Models.GameStatus.WantToPlay,
                BackendGameStatus.Completed => Models.GameStatus.Completed,
                BackendGameStatus.Wishlist => Models.GameStatus.WantToPlay,
                BackendGameStatus.Dropped => Models.GameStatus.Dropped,
                _ => Models.GameStatus.WantToPlay
            };
        }

        private BackendGameStatus MapToBackendStatus(Models.GameStatus frontendStatus)
        {
            return frontendStatus switch
            {
                Models.GameStatus.WantToPlay => BackendGameStatus.Owned,
                Models.GameStatus.Playing => BackendGameStatus.Playing,
                Models.GameStatus.Completed => BackendGameStatus.Completed,
                Models.GameStatus.OnHold => BackendGameStatus.BackLog,
                Models.GameStatus.Dropped => BackendGameStatus.Dropped,
                _ => BackendGameStatus.Owned
            };
        }
    }
}
