﻿using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Infrastructure.Repository
{
    public interface IUnitOfWork
    {
        CatalogContext GetContext { get; }

        IGenericRepository<T> Repository<T>() where T : class;
        void SaveChanges();
        Task<int> SaveChangesAsync();
    }
}