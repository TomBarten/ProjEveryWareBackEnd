// <copyright file="TeacherManager.cs" company="Fvect">
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
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents a reference implementation for an <see cref="ResourceManagerBase{TEntity, TImpl}" />.
    /// </summary>
    public class TeacherManager : ResourceManagerBase<Teacher, TeacherManager>, ITeacherManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeacherManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public TeacherManager(
            FvectContext dbContext,
            ILogger<TeacherManager> logger)
            : base(dbContext, logger)
        {
        }
    }
}
