using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameVault.Domain
{
    public class Game
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime? ReleaseDate { get; set; } = null; // Thay đổi này rất tốt!
        public string Genre { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
        public string[] Platforms { get; set; } = Array.Empty<string>();
       
    }
}
