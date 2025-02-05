using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Db
{
    public class MyDbContextPoolFactory : IMyDbContextPoolFactory
    {
        private readonly IDbContextFactory<TgMediaFileSaverDbContext> contextFactory;

        public MyDbContextPoolFactory(IDbContextFactory<TgMediaFileSaverDbContext> factory)
        {
            contextFactory = factory;
        }

        public IDbContext GetDbContext()
        {
            return contextFactory.CreateDbContext();
        }
    }
}