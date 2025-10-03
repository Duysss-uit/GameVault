# GameVault - Game Collection Manager

A web application for managing your personal video game collection built with Blazor and ASP.NET Core.

## 🎮 Current Features

- ✅ **Game Management**: Add, edit, delete, and view games in your collection
- ✅ **Multiple Views**: Switch between grid and table views
- ✅ **Filtering & Sorting**: Filter by status, sort by various criteria
- ✅ **Status Tracking**: Track games as Playing, Completed, Want to Play, On Hold, or Dropped
- ✅ **Statistics Dashboard**: View insights about your collection (completion rate, playtime, etc.)
- ✅ **Detailed Game Info**: Store genre, platform, rating, playtime, release date, and more
- ✅ **Backend API**: RESTful API with JWT authentication
- ✅ **Database**: PostgreSQL with Entity Framework Core

## 🏗️ Architecture

```
GameVault/
├── GameVault.Web/              # Blazor WebAssembly frontend
├── GameVault.ApiService/       # ASP.NET Core Web API
├── Application/                 # Application layer (services, DTOs)
├── Domain/                      # Domain entities and interfaces
├── Infrastructure/              # Data access (EF Core, repositories)
├── GameVault.AppHost/          # .NET Aspire host
└── GameVault.ServiceDefaults/  # Shared service configurations
```

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL database
- Visual Studio 2022 or VS Code with C# extension

### Building the Project

```bash
# Clone the repository
git clone https://github.com/Duysss-uit/GameVault.git
cd GameVault

# Build the solution
dotnet build

# Run with .NET Aspire (recommended)
cd GameVault.AppHost
dotnet run
```

### Database Setup

The application uses PostgreSQL. Connection string is configured in `appsettings.json` or through Aspire configuration.

```bash
# Apply migrations (if needed)
cd GameVault.ApiService
dotnet ef database update
```

## 📖 Documentation

### For Developers

We've created comprehensive guides to help you enhance this application:

1. **[Missing Features Guide](MISSING_FEATURES_GUIDE.md)**
   - Complete analysis of potential features to add
   - Prioritized by importance (High/Medium/Low)
   - Includes technical improvements and learning opportunities

2. **[Search Implementation Hint](IMPLEMENTATION_HINT_SEARCH.md)**
   - Step-by-step guide to add comprehensive search functionality
   - Includes code patterns and best practices
   - Test cases and UX considerations

3. **[Authentication Implementation Hint](IMPLEMENTATION_HINT_AUTHENTICATION.md)**
   - Detailed guide for implementing login/register UI
   - JWT token management
   - Security best practices

### API Documentation

The API includes Swagger documentation. After running the project, visit:
- **Swagger UI**: `https://localhost:<port>/swagger`

## 🔧 Technology Stack

### Frontend
- **Blazor Server** - Interactive web UI framework
- **Bootstrap 5** - CSS framework
- **Bootstrap Icons** - Icon library

### Backend  
- **ASP.NET Core 9.0** - Web API framework
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **JWT Bearer Authentication** - Security

### DevOps
- **.NET Aspire** - Cloud-native application orchestration
- **Swagger/OpenAPI** - API documentation

## 📝 Key Models

### Game Entity
```csharp
public class Game
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Genre { get; set; }
    public DateTime ReleaseDate { get; set; }
    public decimal Rating { get; set; }
    public GameStatus Status { get; set; }
    public string Platform { get; set; }
    public int PlaytimeHours { get; set; }
    public decimal Price { get; set; }
    // ... more properties
}
```

### Game Status Options
- `WantToPlay` - Games you plan to play
- `Playing` - Currently playing
- `Completed` - Finished games
- `OnHold` - Paused games
- `Dropped` - Abandoned games
- `Wishlist` - Games you want to buy
- `Owned` - Games you own but haven't started

## 🎯 Suggested Next Steps

Based on the analysis in our guides, here are recommended improvements in order of priority:

1. **Add Search Functionality** (High Priority)
   - Enhance user experience with text search
   - Follow the [Search Implementation Hint](IMPLEMENTATION_HINT_SEARCH.md)

2. **Implement Authentication UI** (High Priority)
   - Add login/register pages
   - Backend JWT is ready, needs frontend
   - Follow the [Authentication Implementation Hint](IMPLEMENTATION_HINT_AUTHENTICATION.md)

3. **Advanced Filtering** (Medium Priority)
   - Multi-select filters for Genre, Platform
   - Rating range slider
   - Date range filters

4. **Import/Export** (Medium Priority)
   - Export collection to JSON/CSV
   - Import games from file
   - Backup functionality

5. **Enhanced Statistics** (Low Priority)
   - Interactive charts (Chart.js)
   - Trend analysis
   - Genre diversity visualizations

See [MISSING_FEATURES_GUIDE.md](MISSING_FEATURES_GUIDE.md) for the complete list of potential improvements.

## 🤝 Contributing

This is a learning project, but contributions are welcome! 

### Code Style
- Follow C# coding conventions
- Use meaningful variable names
- Add comments for complex logic
- Write unit tests for new features

### Pull Request Process
1. Create a feature branch
2. Make your changes
3. Test thoroughly
4. Submit PR with clear description

## 📄 License

[Add your license here]

## 🙏 Acknowledgments

Built with:
- [.NET](https://dotnet.microsoft.com/)
- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- [Bootstrap](https://getbootstrap.com/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/)

## 📧 Contact

For questions or suggestions, please open an issue on GitHub.

---

**Happy Gaming! 🎮**
