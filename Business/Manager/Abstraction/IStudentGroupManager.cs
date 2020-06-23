// <copyright file="IStudentGroupManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Abstraction
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Data.Database.Model;

    /// <summary>
    /// Represents an object that can do student group related operations.
    /// </summary>
    public interface IStudentGroupManager : IResourceManager<StudentGroup>
    {
        /// <summary>
        /// Sets the teacher of a student group.
        /// </summary>
        /// <param name="groupId">the id of the group to update.</param>
        /// <param name="teacherId">the id of the teacher to assign to the group.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A value indicating whether the entity was found.</returns>
        Task<bool> SetTeacher(Guid groupId, Guid teacherId, CancellationToken cancellationToken);
    }
}
