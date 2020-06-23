// <copyright file="UserProfileController.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.API.Abstraction;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Common.Models;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Represents a controller for the <see cref="UserProfile"/> resource.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ResourceControllerBase<UserProfile, IResourceManager<UserProfile>>
    {
        private readonly UserManager<AppUser> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileController"/> class.
        /// </summary>
        /// <param name="manager">the manager to use.</param>
        /// <param name="userManager">The user manager.</param>
        public UserProfileController(
            IUserProfileManager manager,
            UserManager<AppUser> userManager)
            : base(manager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <inheritdoc/>
        public override async Task<ActionResult<UserProfile>> GetEntity(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var entity = await this.Manager.FindById(id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return this.NotFound();
            }
            else if (entity.AppUserId != this.GetUserId())
            {
                return this.Forbid();
            }

            return entity;
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutEntity(
            [FromRoute] Guid id,
            [FromBody] UserProfile entity,
            CancellationToken cancellationToken)
        {
            var dbEntity = await this.Manager.FindById(id, cancellationToken).ConfigureAwait(false);

            if (dbEntity is null)
            {
                return this.NotFound();
            }
            else if (dbEntity.AppUserId != this.GetUserId())
            {
                return this.Forbid();
            }

            return await base.PutEntity(id, entity, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override Task<ActionResult> PostEntity(
            [FromBody] UserProfile entity,
            ApiVersion apiVersion,
            CancellationToken cancellationToken)
        {
            entity.AppUserId = this.GetUserId();
            return base.PostEntity(entity, apiVersion, cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task<ActionResult<UserProfile>> DeleteEntity(
            Guid id,
            CancellationToken cancellationToken)
        {
            var dbEntity = await this.Manager.FindById(id, cancellationToken).ConfigureAwait(false);

            if (dbEntity is null)
            {
                return this.NotFound();
            }
            else if (dbEntity.AppUserId != this.GetUserId())
            {
                return this.Forbid();
            }

            return await base.DeleteEntity(id, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override Task<PagedData<UserProfile>> DoGetEntities(int pageSize, int pageIndex, CancellationToken cancellationToken)
            => this.Manager.GetPagedEntitiesPage(
                pageSize,
                pageIndex,
                cancellationToken,
                queryCustomizer: profiles =>
                    profiles
                        .Where(profile =>
                            profile.AppUserId == this.GetUserId()));

        private Guid GetUserId() => Guid.Parse(this.userManager.GetUserId(this.User));
    }
}
