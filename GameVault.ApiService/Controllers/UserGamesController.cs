using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs.UserGameStatus;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGameStatusResponseDto>>> GetVault()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                if (userId == null)
                {
                    return Unauthorized();
                }
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
        public async Task<ActionResult<UserGameStatusResponseDto>> AddGame([FromBody] AddGameToCollectionRequestDto request)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (user == null)
                {
                    return Unauthorized();
                }
                var result = await _userGameStatusService.AddGameToUserVaultAsync(user, request);
                if (result == null)
                {
                    return BadRequest("Failed to add game to vault");
                }
                return CreatedAtAction(nameof(GetUserGameStatusById), new { UserGameStatusId = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding game to user");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("item/{userGameStatusId:guid}", Name = "GetUserGameStatusByID")]
        public async Task<ActionResult<UserGameStatusResponseDto>> GetUserGameStatusById(Guid userGameStatusId)
        {
            var item = await _userGameStatusService.GetUserGameStatusAsync(userGameStatusId);
            if (item == null)
            {
                return NotFound();
            }
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (item.UserId != currentUserId)
            {
                return Forbid();
            }
            return Ok(item);
        }
        [HttpPut("{userGameStatusId:guid}")]
        public async Task<IActionResult> UpdateGameStatus(Guid userGameStatusId, [FromBody] UpdateGameStatusRequestDto request)
        {
            try
            {

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Forbid();
                }
                await _userGameStatusService.UpdateGameStatusInVaultAsync(userGameStatusId, request);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"{User.FindFirstValue(ClaimTypes.NameIdentifier)} try to update game they don't own");
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating status for item {userGameStatusId}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
