using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Db
{
    public class TgMediaFileSaverDbContext : DbContext, IDbContext
    {
        public DbSet<TgUser> TgUsers { get; set; } = null!;
        public DbSet<TgMediaFile> TgMediaFiles { get; set; } = null!;
        public DbSet<InlineResultStatistics> InlineResultStatistics { get; set; } = null!;
        public DbSet<UserAction> UserActions { get; set; } = null!;

        public TgMediaFileSaverDbContext()
        {
        }

        public TgMediaFileSaverDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}