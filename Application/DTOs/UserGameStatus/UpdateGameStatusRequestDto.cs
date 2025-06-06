using Domain.Entities;

namespace Application.DTOs.UserGameStatus
{ 
    public class UpdateGameStatusRequestDto
    {
        public GameStatus NewStatus { get; set; }
    }
}
