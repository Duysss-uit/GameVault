using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs.UserGameStatus;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Authentication;
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
        private readonly IUserGameAuthorizationService _userGameAuthorization;
        public UserGamesController(IUserGameStatusService userGameStatusService, ILogger<UserGamesController> logger, IUserGameAuthorizationService userGameAuthorizationService ,IApplicationDbContext gameService)
        {
            _userGameStatusService = userGameStatusService ?? throw new ArgumentNullException(nameof(userGameStatusService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _userGameAuthorization = userGameAuthorizationService ?? throw new ArgumentNullException(nameof(userGameAuthorizationService));
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
            var own = await _userGameAuthorization.IsUserOwnIt(item.UserId, userGameStatusId);
            if (!own)
            {
                return Forbid();
            }
            try
            {
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{userGameStatusId:guid}")]
        public async Task<IActionResult> UpdateGameStatus(Guid userGameStatusId, [FromBody] UpdateGameStatusRequestDto request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var own = await _userGameAuthorization.IsUserOwnIt(currentUserId, userGameStatusId);
            if (!own)
            {
                return Forbid();
            }
            try
            {
                await _userGameStatusService.UpdateGameStatusInVaultAsync(userGameStatusId, request);
                return NoContent();
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
        [HttpDelete("{userGameStatusId:guid}")]
        public async Task<IActionResult> DeleteGame(Guid userGameStatusId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var owned = await _userGameAuthorization.IsUserOwnIt(currentUserId, userGameStatusId);
            if (!owned)
            {
                return Forbid();
            }
            try
            {
                await _userGameStatusService.RemoveGameFromUserVaultAsync(userGameStatusId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"{User.FindFirstValue(ClaimTypes.NameIdentifier)} try to delete game they don't own");
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting item {userGameStatusId}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
