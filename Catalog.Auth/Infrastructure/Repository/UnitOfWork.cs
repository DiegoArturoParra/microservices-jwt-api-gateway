using Catalog.Auth.Infrastructure;
using Catalog.Auth.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuthContext _context;
    private bool _disposed = false;
    private IDbContextTransaction _currentTransaction;
    public UnitOfWork(AuthContext context)
    {
        _context = context;
    }


    public Dictionary<Type, object> Repositories = new Dictionary<Type, object>();
    public bool HasActiveTransaction => _currentTransaction != null;
    public IGenericRepository<T> Repository<T>() where T : class
    {
        if (Repositories.Keys.Contains(typeof(T)))
        {
            return Repositories[typeof(T)] as IGenericRepository<T>;
        }
        IGenericRepository<T> repo = new GenericRepository<T>(_context);
        Repositories.Add(typeof(T), repo);
        return repo;
    }
    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }
    public void SaveChanges() => _context.SaveChanges();

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Commit() => SaveChanges();

    public async Task CommitAsync()
    {
        if (!HasActiveTransaction)
        {
            throw new InvalidOperationException("No active transaction.");
        }
        await _currentTransaction.CommitAsync();
    }

    public async Task RollbackAsync() => await _currentTransaction.RollbackAsync();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }


}
