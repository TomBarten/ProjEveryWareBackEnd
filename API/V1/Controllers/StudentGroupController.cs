// <copyright file="StudentGroupController.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.API.Abstraction;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Represents a controller for student groups.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StudentGroupController : ResourceControllerBase<StudentGroup, IStudentGroupManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StudentGroupController"/> class.
        /// </summary>
        /// <param name="manager">the manager to use.</param>
        public StudentGroupController(IStudentGroupManager manager)
            : base(manager)
        {
        }

        /// <summary>
        /// Set the teacher of the student group.
        /// </summary>
        /// <param name="groupId">The id of the entity to update.</param>
        /// <param name="teacherId">The id of the teacher to assign.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="Task{TResult}"/>representing the result of the asynchronous operation.</returns>
        /// <response code="204">When the operation completed sucessfully.</response>
        /// <response code="400">When the request is not valid.</response>
        /// <response code="404">When the entity could not be found.</response>
        [HttpPatch("{groupId}/teacher")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetTeacher(
            [FromRoute] Guid groupId,
            [FromBody] Guid teacherId,
            CancellationToken cancellationToken)
        {
            if (groupId == default)
            {
                return this.BadRequest("Cannot update on the default id.");
            }

            if (!await this.Manager.SetTeacher(groupId, teacherId, cancellationToken).ConfigureAwait(false))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
