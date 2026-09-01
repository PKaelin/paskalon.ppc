// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using paskalON.Domains;
using System.Linq.Expressions;

namespace paskalON.Devices.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Generic repository for CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity associated with CRUD operations.</typeparam>
    public class Repository<TContext, TEntity> : IRepository<TContext, TEntity> where TContext : DbContext where TEntity : DomainBase
    {
        /// <summary>
        /// Database context of the generic type.
        /// </summary>
        private readonly TContext _context;


        /// <summary>
        /// Database set of the generic type.
        /// </summary>
        private readonly DbSet<TEntity> _dbSet;


        /// <summary>
        /// Database context of this repository.
        /// </summary>
        public DbContext DatabaseContext
        {
            get { return _context as DbContext; }
        }


        /// <summary>
        /// Constructor of <see cref="Repository"/>.
        /// </summary>
        /// <param name="context">The database context.</param>
        public Repository(TContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            _context = context;
            _dbSet = context.Set<TEntity>();
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        public async Task<Action<TEntity>> CreateAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = await _dbSet.AddAsync(entity);

            return x => x.Id = entry.Entity.Id;
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        public async Task<IEnumerable<TEntity>> GetAsync<TKey>(int skip, int take, Expression<Func<TEntity, TKey>> orderBy,
            bool descending = false, bool trackChanges = false)
        {
            IQueryable<TEntity> query = _dbSet;

            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            query = query.Skip(skip).Take(take);

            if (trackChanges == false)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id) ??
                throw new ApplicationException($"Get {nameof(TEntity)} by id: {id} did not find any entity.");
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>        
        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false)
        {
            IQueryable<TEntity> query = trackChanges ? _dbSet.Where(predicate) : _dbSet.AsNoTracking().Where(predicate);

            return await query.ToListAsync();
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        public TEntity Update(TEntity entity)
        {
            entity.ChangedDate = DateTimeOffset.UtcNow;
            EntityEntry entry = _context.Entry(entity);
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;

            return (TEntity)entry.Entity;
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }


        /// <summary>
        /// <inheritdoc/>>
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Disposes the instance.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
