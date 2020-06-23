// <copyright file="StudentGroupManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents a reference implementation for an <see cref="ResourceManagerBase{TEntity, TImpl}" />.
    /// </summary>
    public class StudentGroupManager : ResourceManagerBase<StudentGroup, StudentGroupManager>, IStudentGroupManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StudentGroupManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public StudentGroupManager(
            FvectContext dbContext,
            ILogger<StudentGroupManager> logger)
            : base(dbContext, logger)
        {
        }

        /// <inheritdoc/>
        public virtual async Task<bool> SetTeacher(Guid groupId, Guid teacherId, CancellationToken cancellationToken)
        {
            var retrievedEntity = await this.FindById(groupId, cancellationToken).ConfigureAwait(false);

            if (retrievedEntity is null)
            {
                return false;
            }

            this.DbContext.Update(retrievedEntity);

            retrievedEntity.TeacherId = teacherId;

            await this.ExecuteDatabaseCall(() => this.DbContext.SaveChangesAsync(cancellationToken)).ConfigureAwait(false);

            return true;
        }
    }
}
