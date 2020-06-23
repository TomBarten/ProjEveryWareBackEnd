// <copyright file="AppUser.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Represents a User of the application.
    /// </summary>
    [SuppressMessage(
        "Usage",
        "CA2227:Collection properties should be read only",
        Justification = "Required by EF Core.")]
    public class AppUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Gets or sets the authentication events of which this user is subjected to.
        /// </summary>
        public List<UserAuthenticationEvent> AuthenticationEvents { get; set; } = null!;

        /// <summary>
        /// Gets or sets the refresh tokens available for this user.
        /// </summary>
        public List<UserRefreshToken> RefreshTokens { get; set; } = null!;
    }
}
