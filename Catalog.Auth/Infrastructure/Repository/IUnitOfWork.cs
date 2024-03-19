namespace Catalog.Auth.Infrastructure.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        Task CommitAsync();
        Task BeginTransactionAsync();
        IGenericRepository<T> Repository<T>() where T : class;
        Task RollbackAsync();
        void SaveChanges();
        Task<int> SaveChangesAsync();
    }
}