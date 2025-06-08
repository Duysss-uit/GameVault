using Application.DTOs.UserGameStatus;
using Application.Interfaces;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.Service
{
    public class UserGameStatusService : IUserGameStatusService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UserGameStatusService> _logger;
        public UserGameStatusService(IApplicationDbContext context, ILogger<UserGameStatusService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task UpdateNotesAsync(Guid userGameStatusId, string? notes)
        {
            var entry = await _context.UserGameStatuses.FindAsync(userGameStatusId);
            if (entry == null)
            {
                _logger.LogWarning($"UserGameStatus with Id {userGameStatusId} not found.");
                return;
            }
            entry.Notes = notes;
            entry.DateModified = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        public async Task UpdateGameStatusInVaultAsync(Guid userGameStatusId, UpdateGameStatusRequestDto requestDto)
        {
            var entry = await _context.UserGameStatuses.FindAsync(userGameStatusId);
            if (entry == null)
            {
                _logger.LogWarning($"UserGameStatus with Id {userGameStatusId} not found.");
                return;
            }
            entry.UpdateStatus(requestDto.NewStatus);
            entry.DateModified = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        public async Task UpdateGameRatingInVaultAsync(Guid userGameStatusId, int? newRating)
        {
            var entry = await _context.UserGameStatuses.FindAsync(userGameStatusId);
            if (entry == null)
            {
                _logger.LogWarning($"UserGameStatus with Id {userGameStatusId} not found.");
                return;
            }
            entry.UpdateRating(newRating);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveGameFromUserVaultAsync(Guid userGameStatusId)
        {
            var entry = await _context.UserGameStatuses.FindAsync(userGameStatusId);
            if (entry == null)
            {
                _logger.LogWarning($"UserGameStatus with Id {userGameStatusId} not found.");
                return;
            }
            _context.UserGameStatuses.Remove(entry);
            await _context.SaveChangesAsync();
        }

        // 6. GetUserGameStatusAsync: Trả về UserGameStatusResponseDto
        public async Task<UserGameStatusResponseDto?> GetUserGameStatusAsync(Guid userGameStatusId)
        {
            var entry = await _context.UserGameStatuses
                .Include(ugs => ugs.Game)
                .FirstOrDefaultAsync(ugs => ugs.Id == userGameStatusId);
            return entry != null ? new UserGameStatusResponseDto
            {
                Id = entry.Id,
                UserId = entry.UserId,
                GameId = entry.GameId,
                GameName = entry.Game?.Name ?? "unknowned game",
                GameCoverImageUrl = entry.Game?.CoverImageUrl ?? "unknowned game",
                GamePlatforms = entry.Game?.Platforms,
                Status = entry.Status,
                DateAdded = entry.DateAdded,
                PersonalRating = entry.PersonalRating,
                DateModified = entry.DateModified,
                Notes = entry.Notes
            } : null;
        }
        public async Task<IEnumerable<UserGameStatusResponseDto>> GetUserCollectionAsync(string userId)
        {
            var userGameStatuses = await _context.UserGameStatuses
                .Where(ugs => ugs.UserId == userId)
                .Include(ugs => ugs.Game) // Include Game entity to get game details
                .ToListAsync(); // Fetch the data asynchronously

            return userGameStatuses.Select(ugs => new UserGameStatusResponseDto
            {
                Id = ugs.Id,
                UserId = ugs.UserId,
                GameId = ugs.GameId,
                GameName = ugs.Game?.Name ?? "unknowed game",
                GameCoverImageUrl = ugs.Game?.CoverImageUrl,
                GamePlatforms = ugs.Game?.Platforms,
                Status = ugs.Status,
                DateAdded = ugs.DateAdded,
                PersonalRating = ugs.PersonalRating,
                DateModified = ugs.DateModified,
                Notes = ugs.Notes
            }).ToList();
        }
        public async Task<UserGameStatusResponseDto?> AddGameToUserVaultAsync(string userId, AddGameToCollectionRequestDto requestDto)
        {
            var game = await _context.Games.FindAsync(requestDto.GameId);
            if (game == null)
            {
                throw new KeyNotFoundException($"Game with Id {requestDto.GameId} not found.");
            }
            UserGameStatus? entryThatWasProcessed = null; // Biến để lưu trữ entity sẽ được xử lý (mới hoặc cập nhật)
            var existingEntry = await _context.UserGameStatuses
                .FirstOrDefaultAsync(ugs => ugs.UserId == userId && ugs.GameId == requestDto.GameId);
            if (existingEntry != null)
            {
                _logger.LogInformation($"Already has game {requestDto.GameId} in collection.");
                existingEntry.Status = requestDto.InitialStatus; // Luôn cập nhật theo trạng thái mới từ request
                existingEntry.PersonalRating = requestDto.PersonalRating;
                existingEntry.Notes = requestDto.Notes;
                existingEntry.DateModified = DateTime.UtcNow; // Luôn cập nhật DateModified
                entryThatWasProcessed = existingEntry;
            }
            else
            {
                var newEntry = new UserGameStatus
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    GameId = requestDto.GameId,
                    Status = requestDto.InitialStatus,
                    PersonalRating = requestDto.PersonalRating,
                    Notes = requestDto.Notes,
                    DateAdded = DateTime.UtcNow,
                    DateModified = null
                };
                _context.UserGameStatuses.Add(newEntry);
                entryThatWasProcessed = newEntry;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"error update database for UserId {userId}, GameId {requestDto.GameId}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"error saving UserGameStatus for UserId {userId}, GameId {requestDto.GameId}", ex);
            }
            return new UserGameStatusResponseDto
            {
                Id = entryThatWasProcessed.Id,
                UserId = entryThatWasProcessed.UserId,
                GameId = entryThatWasProcessed.GameId,
                GameName = game.Name,
                GameCoverImageUrl = game.CoverImageUrl,
                GamePlatforms = game.Platforms,
                Status = entryThatWasProcessed.Status,
                DateAdded = entryThatWasProcessed.DateAdded,
                PersonalRating = entryThatWasProcessed.PersonalRating,
                DateModified = entryThatWasProcessed.DateModified,
                Notes = entryThatWasProcessed.Notes
            };
        }
    }
}
