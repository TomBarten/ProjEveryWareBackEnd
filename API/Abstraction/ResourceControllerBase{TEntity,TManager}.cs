// <copyright file="ResourceControllerBase{TEntity,TManager}.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.Abstraction
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Common.Models;
    using Fvect.Backend.Data.Database.Base;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Represents generic a controller for resource entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TManager">The manager type.</typeparam>
    /// <remarks>
    /// Inheritors should manually add the <see cref="RouteAttribute"/> and the
    /// <see cref="ApiControllerAttribute"/> to their class.
    /// </remarks>
    public abstract class ResourceControllerBase<TEntity, TManager> : ControllerBase
        where TEntity : ResourceEntityBase<TEntity>, new()
        where TManager : class, IResourceManager<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceControllerBase{TEntity, TManager}"/> class.
        /// </summary>
        /// <param name="manager">The manager to use.</param>
        public ResourceControllerBase(TManager manager)
        {
            this.Manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        /// <summary>
        /// Gets the manager associated with this <see cref="ResourceControllerBase{TEntity, TManager}"/>.
        /// </summary>
        protected TManager Manager { get; }

        /// <summary>
        /// Gets a page of entities.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <param name="pageSize">The amount of entities per page.</param>
        /// <param name="pageIndex">The index of the page.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> representing the result of the asynchronous operation.</returns>
        /// <response code="200">When the operation completed successfully.</response>
        /// <response code="400">When the request was not valid.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<PagedData<TEntity>>> GetEntities(
            CancellationToken cancellationToken,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0)
        {
            if (pageSize < 1)
            {
                return this.BadRequest($"'{nameof(pageSize)}' may not be less then 1.");
            }

            if (pageIndex < 0)
            {
                return this.BadRequest($"'{nameof(pageIndex)}' may not be less then 0.");
            }

            return await this.DoGetEntities(pageSize, pageIndex, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a specific entity by id.
        /// </summary>
        /// <param name="id">The id of the requested entity.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>The requested entity.</returns>
        /// <response code="200">When the operation completed successfully.</response>
        /// <response code="404">
        /// When the request requested an entity that does not exist.
        /// </response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TEntity>> GetEntity(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var entity = await this.Manager.FindById(id, cancellationToken).ConfigureAwait(false);

            if (entity == null)
            {
                return this.NotFound();
            }

            return entity;
        }

        /// <summary>
        /// Update an entity.
        /// </summary>
        /// <param name="id">The id of the entity to update.</param>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="Task{TResult}"/>representing the result of the asynchronous operation.</returns>
        /// <response code="204">When the operation completed sucessfully.</response>
        /// <response code="400">When the request is not valid.</response>
        /// <response code="404">When the entity could not be found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> PutEntity(
            [FromRoute] Guid id,
            [FromBody] TEntity entity,
            CancellationToken cancellationToken)
        {
            if (id == default)
            {
                return this.BadRequest("Cannot update on the default id.");
            }

            if (id != (entity ?? throw new ArgumentNullException(nameof(entity))).Id)
            {
                entity.Id = id;
            }

            if (!await this.Manager.Update(entity, cancellationToken).ConfigureAwait(false))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        /// <summary>
        /// Adds an entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="apiVersion">The current api version.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A redirect to the added entity.</returns>
        /// <response code="201">When the operation completed sucessfully.</response>
        /// <response code="400">When the request was not valid.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult> PostEntity(
            [FromBody] TEntity entity,
            ApiVersion apiVersion,
            CancellationToken cancellationToken)
        {
            if ((entity ?? throw new ArgumentNullException(nameof(entity))).Id != default)
            {
                return this.BadRequest("Entities to be added may not specify an identifier.");
            }

            var id = await this.Manager.Add(entity, cancellationToken).ConfigureAwait(false);

            return this.CreatedAtAction(
                nameof(this.GetEntity),
                new
                {
                    id,
                    version = (apiVersion ?? throw new ArgumentNullException(nameof(apiVersion))).ToString(),
                },
                null);
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="id">The id of the entity to delete.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="Task{TResult}"/>representing the result of the asynchronous operation.</returns>
        /// <response code="204">When the operation completed sucessfully.</response>
        /// <response code="404">
        /// When the entity does not exist.
        /// </response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<TEntity>> DeleteEntity(
            Guid id,
            CancellationToken cancellationToken)
        {
            var succes = await this.Manager.Remove(id, cancellationToken).ConfigureAwait(false);

            if (!succes)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        /// <summary>
        /// The method that is called to actually retrieve the paged entities. This allows for the retrieval logic
        /// to be overriden but the validation logic to remain in place.
        /// </summary>
        /// <param name="pageSize">The amount of entities per page.</param>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>The retreived entities.</returns>
        protected virtual Task<PagedData<TEntity>> DoGetEntities(
            int pageSize,
            int pageIndex,
            CancellationToken cancellationToken)
        {
            return this.Manager.GetPagedEntitiesPage(
                pageSize, pageIndex, cancellationToken);
        }
    }
}
