using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameVault.Domain;


namespace Application.DTOs.UserGameStatus
{
    // Nhớ using GameVaultManager.Domain; ở đầu file
    // Namespace: GameVaultManager.Application.DTOs.Collection (hoặc UserGameStatus)
    public class UserGameStatusResponseDto
    {
        public Guid Id { get; set; } // Id của bản ghi UserGameStatus
                                     // public string UserId { get; set; } // Có thể không cần trả UserId về client nếu client đã biết
        public string GameId { get; set; }
        public string GameName { get; set; } // Thông tin lấy từ Game entity
        public string? GameCoverImageUrl { get; set; } // Thông tin lấy từ Game entity
        public string[]? GamePlatforms { get; set; } // Thông tin lấy từ Game entity
        public GameStatus Status { get; set; }
        public DateTime DateAdded { get; set; }
        public int? PersonalRating { get; set; }
        public DateTime? DateModified { get; set; }
        public string? Notes { get; set; }
        // Bạn có thể thêm các thuộc tính khác nếu thấy cần thiết cho việc hiển thị
    }
}
