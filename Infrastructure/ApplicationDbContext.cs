using GameVault.Domain;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {}
        public DbSet<Game> Games { get; set; }
        public DbSet<UserGameStatus> UserGameStatuses { get; set; }
    }
}
