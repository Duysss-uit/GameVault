using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// using System.Security.Cryptography.X509Certificates; // (Có vẻ không cần thiết ở đây)

namespace Domain.Entities
{
    public class UserGameStatus // 1. Access Modifier (internal)
    {
        public string UserId { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public Game? Game { get; set; } // 2. Game entity - Tuyệt vời! Bạn đã sử dụng Game entity để liên kết với UserGameStatus - Rất tốt!
        public Guid Id { get; set; } = Guid.NewGuid(); // Thêm Id cho UserGameStatus - Tuyệt vời!
        public GameStatus Status { get; set; } = GameStatus.Owned; // Có giá trị khởi tạo mặc định - Hợp lý!
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public int? PersonalRating { get; set; } = null; // Khởi tạo PersonalRating là null - Chính xác!
        public DateTime? DateModified { get; set; } = null; // Khởi tạo DateModified là null - Rất tốt!
        public string? Notes { get; set; } = null;

        public void UpdateRating(int? newRating)
        {
            PersonalRating = newRating;
            DateModified = DateTime.UtcNow;
        }

        public void UpdateStatus(GameStatus newStatus)
        {
            Status = newStatus;
            DateModified = DateTime.UtcNow;
        }
    }
}
