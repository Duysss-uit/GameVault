namespace GameVault.Web.DTOs
{
    public class UserGameStatusResponseDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public string? GameDescription { get; set; }
        public string? GameDeveloper { get; set; }
        public string? GamePublisher { get; set; }
        public DateTime? GameReleaseDate { get; set; }
        public string? GameGenre { get; set; }
        public string? GameCoverImageUrl { get; set; }
        public string? GameTrailerUrl { get; set; }
        public string[]? GamePlatforms { get; set; }
        public BackendGameStatus Status { get; set; }
        public DateTime DateAdded { get; set; }
        public int? PersonalRating { get; set; }
        public DateTime? DateModified { get; set; }
        public string? Notes { get; set; }
    }

    public class AddGameToCollectionRequestDto
    {
        public string GameId { get; set; } = string.Empty;
        public BackendGameStatus InitialStatus { get; set; } = BackendGameStatus.Owned;
        public int? PersonalRating { get; set; }
        public string? Notes { get; set; }
        public BackendGame? game { get; set; }
    }

    public class UpdateGameStatusRequestDto
    {
        public BackendGameStatus NewStatus { get; set; }
    }

    public class BackendGame
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime? ReleaseDate { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
        public string[] Platforms { get; set; } = Array.Empty<string>();
    }

    public enum BackendGameStatus
    {
        Owned,
        Playing,
        BackLog,
        Completed,
        Wishlist,
        Dropped
    }
}
