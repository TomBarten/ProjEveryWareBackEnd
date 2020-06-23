// <copyright file="IResourceManager{TEntity}.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Abstraction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Common.Models;
    using Fvect.Backend.Data.Database.Base;

    /// <summary>
    /// Specifies a manager for a <see cref="ResourceEntityBase{TEntity}"/> resource.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public interface IResourceManager<TEntity>
        where TEntity : ResourceEntityBase<TEntity>, new()
    {
        /// <summary>
        /// Gets an entity by id, or null.
        /// </summary>
        /// <param name="id">The identifier of the required entity.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>The requested entity, or <c>null</c>.</returns>
        ValueTask<TEntity?> FindById(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a page of entities.
        /// </summary>
        /// <param name="pageSize">Amount of entities to get per page.</param>
        /// <param name="pageIndex">The index of the page to retrieve.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <param name="queryCustomizer">
        /// A customizer <see cref="Func{T, TResult}" /> for the query.
        /// This allows the caller to, for example, add filters or load relationships.
        /// </param>
        /// <returns>The paged entities.</returns>
        Task<PagedData<TEntity>> GetPagedEntitiesPage(
            int pageSize,
            int pageIndex,
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryCustomizer = null);

        /// <summary>
        /// Adds an entity.
        /// </summary>
        /// <param name="newEntity">A new entity to be added.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>The key of the newly created resource.</returns>
        Task<Guid> Add(TEntity newEntity, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a range of entities.
        /// </summary>
        /// <param name="newEntities">A range of new entities to be added.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>The keys of the newly created resources.</returns>
        Task<IEnumerable<Guid>> AddRange(IEnumerable<TEntity> newEntities, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A value indicating whether the entity was found.</returns>
        Task<bool> Update(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A value indicating whether the entity was found.</returns>
        Task<bool> Remove(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a range of entities.
        /// </summary>
        /// <param name="entityIds">A range of entities to be removed.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="Task"/> respresenting the asynchronous operation.</returns>
        Task RemoveRange(IEnumerable<Guid> entityIds, CancellationToken cancellationToken);
    }
}
