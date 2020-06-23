// <copyright file="UserProfileManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Implementation
{
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents the manager for the <see cref="UserProfile"/> entity.
    /// </summary>
    public class UserProfileManager : ResourceManagerBase<UserProfile, UserProfileManager>, IUserProfileManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public UserProfileManager(
            FvectContext dbContext,
            ILogger<UserProfileManager> logger)
            : base(dbContext, logger)
        {
        }
    }
}
