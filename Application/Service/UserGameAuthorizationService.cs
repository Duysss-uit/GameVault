using Microsoft.Extensions.Logging;
using Application.Interfaces;
namespace Application.Service;

public class UserGameAuthorizationService : IUserGameAuthorizationService
{
    private readonly UserGameStatusService _userGameStatusService;
    private readonly ILogger<UserGameAuthorizationService> _logger;
    public UserGameAuthorizationService(UserGameStatusService userstatus, ILogger<UserGameAuthorizationService> logger)
    {
        _userGameStatusService = userstatus ?? throw new ArgumentNullException(nameof(userstatus)); ;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task<bool> IsUserOwnIt(string userId, Guid gameId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }
        try
        {
            var game = await _userGameStatusService.GetUserGameStatusAsync(gameId);
            if (game == null)
            {
                return false;
            }
            if (game.UserId == userId)
            {
                _logger.LogDebug($"User {userId} owns game {gameId}");
                return true;
            }
            else
            {
                _logger.LogWarning($"User {userId} try to access game {gameId} of {game.UserId}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"System error during authorization check for user {userId}, game {gameId}", ex, userId, gameId);
            return false;
        }
    }
}
