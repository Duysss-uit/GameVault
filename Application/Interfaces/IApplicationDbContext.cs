// Trong GameVault.Application/Interfaces/IApplicationDbContext.cs (ví dụ)
using Domain.Entities;
using Domain; // Hoặc GameVaultManager.Domain tùy namespace của bạn
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GameVault.Application.Interfaces // Hoặc GameVaultManager.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Game> Games { get; set; }
        DbSet<UserGameStatus> UserGameStatuses { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        // Thêm các phương thức hoặc thuộc tính khác của DbContext mà Application services cần
    }
}