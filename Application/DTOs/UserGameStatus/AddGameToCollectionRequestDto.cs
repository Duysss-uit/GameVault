using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameVault.Domain;

namespace Application.DTOs.UserGameStatus
{
    // Nhớ using GameVaultManager.Domain;
    // Namespace: GameVaultManager.Application.DTOs.Collection (hoặc UserGameStatus)
    public class AddGameToCollectionRequestDto
    {
        // UserId thường sẽ được lấy từ context của người dùng đã đăng nhập ở phía API Controller,
        // nên có thể không cần client gửi lên trực tiếp trong DTO này.
        public string GameId { get; set; } // ID của game muốn thêm
        public GameStatus InitialStatus { get; set; } = GameStatus.Owned; // Trạng thái ban đầu
        public int? PersonalRating { get; set; } // Đánh giá cá nhân (tùy chọn)
        public string? Notes { get; set; } // Ghi chú cá nhân (tùy chọn)
    }
}
