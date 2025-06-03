using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;

namespace Application.DTOs.UserGameStatus
{ 
    public class UpdateGameStatusRequestDto
    {
        public GameStatus NewStatus { get; set; }
    }
}
