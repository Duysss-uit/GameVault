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
    public class UpdateGameStatusRequestDto
    {
        public GameStatus NewStatus { get; set; }
    }
}
