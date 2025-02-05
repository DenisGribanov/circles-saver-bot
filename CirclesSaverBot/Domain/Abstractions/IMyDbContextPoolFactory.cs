namespace Domain.Abstractions
{
    public interface IMyDbContextPoolFactory
    {
        IDbContext GetDbContext();
    }
}