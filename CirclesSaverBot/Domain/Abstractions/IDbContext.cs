using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.Abstractions
{
    public interface IDbContext : IDisposable
    {
        DbSet<TgUser> TgUsers { get; set; }

        DbSet<TgMediaFile> TgMediaFiles { get; set; }

        DbSet<InlineResultStatistics> InlineResultStatistics { get; set; }

        DbSet<UserAction> UserActions { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}