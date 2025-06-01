using Application.DTOs.UserGameStatus;
using Domain;
using GameVault.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameVault.Application
{
    public interface IUserGameStatusService
    {
        // 1. AddGameToUserVaultAsync: Nhận AddGameToCollectionRequestDto, trả về UserGameStatusResponseDto
        Task<UserGameStatusResponseDto?> AddGameToUserVaultAsync(string userId, AddGameToCollectionRequestDto requestDto);

        // 2. UpdateGameStatusInVaultAsync: Nhận UpdateGameStatusRequestDto
        Task UpdateGameStatusInVaultAsync(Guid userGameStatusId, UpdateGameStatusRequestDto requestDto);

        // 3. UpdateGameRatingInVaultAsync: Có thể giữ nguyên hoặc dùng DTO riêng nếu muốn
        //    Hiện tại, để đơn giản, chúng ta giữ nguyên nếu chỉ cập nhật rating.
        Task UpdateGameRatingInVaultAsync(Guid userGameStatusId, int? newRating);

        // 4. UpdateNotesAsync: Tương tự, có thể giữ nguyên hoặc dùng DTO riêng
        Task UpdateNotesAsync(Guid userGameStatusId, string? notes);

        // 5. RemoveGameFromUserVaultAsync: Giữ nguyên, không cần DTO phức tạp
        Task RemoveGameFromUserVaultAsync(Guid userGameStatusId);

        // 6. GetUserGameStatusAsync: Trả về UserGameStatusResponseDto
        Task<UserGameStatusResponseDto?> GetUserGameStatusAsync(Guid userGameStatusId);

        // 7. GetUserCollectionAsync: Trả về một danh sách UserGameStatusResponseDto
        Task<IEnumerable<UserGameStatusResponseDto>> GetUserCollectionAsync(string userId);
    }
}