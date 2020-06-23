// <copyright file="ResourceManagerBase{TEntity,TImpl}.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Common.Exception;
    using Fvect.Backend.Common.Models;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Base;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents a base class for managers managing a <see cref="ResourceEntityBase{TEntity}" />.
    /// </summary>
    /// <typeparam name="TEntity">The entity managed by this manager.</typeparam>
    /// <typeparam name="TImpl">The implementation type of this manager.</typeparam>
    public abstract class ResourceManagerBase<TEntity, TImpl> : IResourceManager<TEntity>
        where TEntity : ResourceEntityBase<TEntity>, new()
        where TImpl : ResourceManagerBase<TEntity, TImpl>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManagerBase{TEntity, TImpl}"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        protected ResourceManagerBase(
            FvectContext dbContext,
            ILogger<TImpl> logger)
        {
            this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.DbSet = dbContext.Set<TEntity>();
        }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected FvectContext DbContext { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger<TImpl> Logger { get; }

        /// <summary>
        /// Gets the database set for the entity type.
        /// </summary>
        protected DbSet<TEntity> DbSet { get; }

        /// <inheritdoc/>
        public virtual ValueTask<TEntity?> FindById(Guid id, CancellationToken cancellationToken)
#nullable disable // Type within the generic is not nullable when returned from EF core.
            => this.DbSet.FindAsync(new object[] { id }, cancellationToken);
#nullable restore

        /// <inheritdoc/>
        public async Task<PagedData<TEntity>> GetPagedEntitiesPage(
            int pageSize,
            int pageIndex,
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryCustomizer = null)
        {
            if (pageSize < 1)
            {
                throw new ArgumentException("Must be greater then or equal to 1.", nameof(pageSize));
            }

            if (pageIndex < 0)
            {
                throw new ArgumentException("Must be greater then or equal to 0.", nameof(pageIndex));
            }

            IQueryable<TEntity> query = this.DbSet;

            if (!(queryCustomizer is null))
            {
                query = queryCustomizer(query);
            }

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var totalPageCount = (int)Math.Ceiling((double)totalCount / pageSize);

            var content = await query
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync()
                .ConfigureAwait(false);

            return new PagedData<TEntity>(
                pageIndex,
                totalPageCount,
                pageSize,
                content);
        }

        /// <inheritdoc/>
        public virtual async Task<Guid> Add(TEntity newEntity, CancellationToken cancellationToken)
        {
            if (newEntity is null)
            {
                throw new ArgumentNullException(nameof(newEntity));
            }

            if (newEntity.Id != default)
            {
                throw new ArgumentException("Entities that are to be added to the database should not have an id defined.");
            }

            var entry = this.DbSet.Add(newEntity);

            await this.ExecuteDatabaseCall(() => this.DbContext.SaveChangesAsync(cancellationToken)).ConfigureAwait(false);

            return entry.Entity.Id;
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<Guid>> AddRange(IEnumerable<TEntity> newEntities, CancellationToken cancellationToken)
        {
            if (newEntities == null)
            {
                throw new ArgumentNullException(nameof(newEntities));
            }

            var entityList = newEntities.ToImmutableList();

            if (entityList.Any(x => x.Id != default))
            {
                throw new ArgumentException("Entities that are to be added to the database should not have an id defined.");
            }

            this.DbSet.AddRange(entityList);

            await this.ExecuteDatabaseCall(() => this.DbContext.SaveChangesAsync(cancellationToken)).ConfigureAwait(false);

            return entityList.Select(x => x.Id);
        }

        /// <inheritdoc/>
        public virtual async Task<bool> Update(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var retrievedEntity = await this.FindById(entity.Id, cancellationToken).ConfigureAwait(false);

            if (retrievedEntity is null)
            {
                return false;
            }

            this.DbContext.Entry(retrievedEntity).CurrentValues.SetValues(entity);

            await this.ExecuteDatabaseCall(() => this.DbContext.SaveChangesAsync(cancellationToken)).ConfigureAwait(false);

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> Remove(Guid id, CancellationToken cancellationToken = default)
        {
            var retrievedEntity = await this.FindById(id, cancellationToken).ConfigureAwait(false);

            if (retrievedEntity is null)
            {
                return false;
            }

            this.DbSet.Remove(retrievedEntity);

            await this.ExecuteDatabaseCall(() => this.DbContext.SaveChangesAsync(cancellationToken)).ConfigureAwait(false);

            return true;
        }

        /// <inheritdoc/>
        public async Task RemoveRange(IEnumerable<Guid> entityIds, CancellationToken cancellationToken)
        {
            this.DbSet.RemoveRange(entityIds.Select(id => new TEntity { Id = id }));
            await this.ExecuteDatabaseCall(() => this.DbContext.SaveChangesAsync(cancellationToken)).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a call to the database, returning the result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <returns>The result.</returns>
        protected async Task<TResult> ExecuteDatabaseCall<TResult>(Func<Task<TResult>> operation)
        {
            try
            {
                return await (operation ?? throw new ArgumentNullException(nameof(operation)))().ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException cEx)
            {
                throw new DataConflictException("A database concurrency conflict occurred.", cEx);
            }
        }
    }
}
