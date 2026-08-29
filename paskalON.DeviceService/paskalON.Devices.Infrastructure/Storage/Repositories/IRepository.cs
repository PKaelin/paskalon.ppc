// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Domains;
using System.Linq.Expressions;

namespace paskalON.Devices.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// A generic repository facilitating CRUD requests to a data context.
    /// </summary>
    public interface IRepository<TContext, TEntity> where TEntity : DomainBase
    {
        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The new entity.</returns>
        Task<Action<TEntity>> CreateAsync(TEntity entity);


        /// <summary>
        /// Paged entities.
        /// </summary>
        /// <param name="skip">How many to skip in the page.</param>
        /// <param name="take">How many to take in the page.</param>
        /// <param name="trackChanges">Flag whether to track the entities or not.</param>
        /// <returns>All entities within a page.</returns>
        Task<IEnumerable<TEntity>> GetAsync(int skip, int take, bool trackChanges = false);


        /// <summary>
        /// Get entity by id.
        /// </summary>
        /// <param name="id">Id of entity to get.</param>
        /// <returns>The entity.</returns>
        Task<TEntity> GetByIdAsync(int id);


        /// <summary>
        /// Get a list of entities that match a predict.
        /// </summary>
        /// <example>
        /// (e) => e.ChangedBy == "User"
        /// </example>
        /// <param name="predicate">Predict of the query.</param>
        /// <param name="trackChanges">Flag whether to track the entities or not.</param>
        /// <returns>List of entities.</returns>
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false);


        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <returns>The updated entity.</returns>
        TEntity Update(TEntity entity);


        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        void Delete(TEntity entity);


        /// <summary>
        /// Saves the changes to the context.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
