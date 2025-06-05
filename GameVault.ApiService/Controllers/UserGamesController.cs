using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Service;
using Application.DTOs.UserGameStatus;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace GameVault.ApiService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserGamesController : ControllerBase
    {
        private readonly IUserGameStatusService _userGameStatusService;
        private readonly ILogger<UserGamesController> _logger;
        private readonly IApplicationDbContext _gameService;
        public UserGamesController(IUserGameStatusService userGameStatusService, ILogger<UserGamesController> logger, IApplicationDbContext gameService)
        {
            _userGameStatusService = userGameStatusService ?? throw new ArgumentNullException(nameof(userGameStatusService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<UserGameStatusResponseDto>>> GetVaultByID()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _userGameStatusService.GetUserCollectionAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving games of user {userId}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddGame([FromBody] AddGameToCollectionRequestDto request)
        {
            try
            {
                var gameExists = await _gameService.Games.AnyAsync(g => g.Id == request.GameId);
                if (!gameExists)
                {
                    return NotFound("Game not found");
                }
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _userGameStatusService.AddGameToUserVaultAsync(user, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding game to user");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
