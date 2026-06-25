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
        /// Gets the database context of this repository.
        /// </summary>
        protected DbContext DatabaseContext
        {
            get { return _context as DbContext; }
        }


        /// <summary>
        /// Constructor of <see cref="Repository"/>.
        /// </summary>
        /// <param name="context">The database context.</param>
        protected Repository(TContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            _context = context;
            _dbSet = context.Set<TEntity>();
        }


        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The new entity.</returns>
        public async Task<Action<TEntity>> CreateAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = await _dbSet.AddAsync(entity);

            return x => x.Id = entry.Entity.Id;
        }


        /// <summary>
        /// Gets paged entities.
        /// </summary>
        /// <param name="skip">How many to skip in the page.</param>
        /// <param name="take">How many to take in the page.</param>
        /// <param name="trackChanges">Flag whether to track the entities or not.</param>
        /// <returns>All entities within a page.</returns>
        public async Task<IEnumerable<TEntity>> GetAsync(int skip, int take, bool trackChanges = false)
        {
            IQueryable<TEntity> query = trackChanges ? _dbSet.Skip(skip).Take(take) : _dbSet.Skip(skip).Take(take).AsNoTracking();

            return await query.ToListAsync();
        }


        /// <summary>
        /// Get entity by id.
        /// </summary>
        /// <param name="id">Id of entity to get.</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Throws an exception if entity is not found.</exception>
        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id) ??
                throw new ApplicationException($"Get {nameof(TEntity)} by id: {id} did not find any entity.");
        }


        /// <summary>
        /// Get a list of entities that match a predict.
        /// </summary>
        /// <example>
        /// (e) => e.ChangedBy == "User"
        /// </example>
        /// <param name="predicate">Predict of the query.</param>
        /// <param name="trackChanges">Flag whether to track the entities or not.</param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false)
        {
            IQueryable<TEntity> query = trackChanges ? _dbSet.Where(predicate) : _dbSet.AsNoTracking().Where(predicate);

            return await query.ToListAsync();
        }


        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <returns>Returns the updated entity.</returns>
        public TEntity Update(TEntity entity)
        {
            entity.ChangedDate = DateTimeOffset.UtcNow;
            EntityEntry entry = _context.Entry(entity);
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;

            return (TEntity)entry.Entity;
        }


        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }


        /// <summary>
        /// Saves the changes to the context.
        /// </summary>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
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
