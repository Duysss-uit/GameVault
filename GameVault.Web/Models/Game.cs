using System.ComponentModel.DataAnnotations;

namespace GameVault.Web.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Game title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters")]
        public string Genre { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10")]
        public decimal Rating { get; set; }

        public GameStatus Status { get; set; }

        public DateTime DateAdded { get; set; }

        [StringLength(50, ErrorMessage = "Platform cannot exceed 50 characters")]
        public string Platform { get; set; } = string.Empty;

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string CoverImageUrl { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public bool IsCompleted { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CompletionDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Playtime must be a positive value")]
        public int PlaytimeHours { get; set; }
    }

    public enum GameStatus
    {
        WantToPlay = 0,
        Playing = 1,
        Completed = 2,
        OnHold = 3,
        Dropped = 4
    }
}
