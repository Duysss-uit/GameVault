using GameVault.Web.Services;
using GameVault.Web.Models;
using Xunit;
using System.Linq;
using System.Threading.Tasks;

namespace GameVault.Web.Tests
{
    public class MockGameServiceTests
    {
        [Fact]
        public async Task SearchGamesAsync_ShouldReturnGames_WhenSearchingByDeveloper()
        {
            // Arrange
            var service = new MockGameService();
            var searchTerm = "Nintendo EPD";

            // Act
            var result = await service.SearchGamesAsync(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("The Legend of Zelda: Breath of the Wild", result.First().Title);
        }

        [Fact]
        public async Task SearchGamesAsync_ShouldReturnGames_WhenSearchingByPublisher()
        {
            // Arrange
            var service = new MockGameService();
            var searchTerm = "CD Projekt";

            // Act
            var result = await service.SearchGamesAsync(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Cyberpunk 2077", result.First().Title);
        }

        [Fact]
        public async Task SearchGamesAsync_ShouldNotThrowException_ForPartialMatches()
        {
            // Arrange
            var service = new MockGameService();
            var searchTerm = "Nintendo";

            // Act
            var result = await service.SearchGamesAsync(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }
}