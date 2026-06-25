using paskalON.Domains;

namespace paskalON.Devices.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// A generic repository facilitating CRUD requests to a data context.
    /// </summary>
    public interface IRepository<TContext, TEntity> where TEntity : DomainBase
    {
        //Task<List<TEntity>> GetAllAsync();
        //Task<TEntity> GetAsync(int id);
        //Task<TEntity> AddAsync(TEntity entity);
        //Task<TEntity> UpdateAsync(TEntity entity);
        //Task<TEntity> DeleteAsync(int id);
        //Task<int> SaveChangesAsync();
    }
}
