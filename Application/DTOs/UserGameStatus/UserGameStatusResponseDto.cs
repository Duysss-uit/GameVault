using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;


namespace Application.DTOs.UserGameStatus
{
    // Nhớ using GameVaultManager.Domain; ở đầu file
    // Namespace: GameVaultManager.Application.DTOs.Collection (hoặc UserGameStatus)
    public class UserGameStatusResponseDto
    {
        public Guid Id { get; set; } // Id của bản ghi UserGameStatus
        public required string UserId { get; set; } // Id của người dùng sở hữu game                             
        public required string GameId { get; set; }
        public required string GameName { get; set; } // Thông tin lấy từ Game entity
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
