// <copyright file="AuthNROptions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Options
{
    /// <summary>
    /// Represents the options for authentication and authorization.
    /// </summary>
    public class AuthNROptions
    {
        /// <summary>
        /// Gets or sets the key used for signing and validating requests. Should be
        /// stored as a user secret or in a key vault and must be at least 16 characters long.
        /// </summary>
        public string JWTSigningKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the authority of the JWT.
        /// </summary>
        public string JWTAuthority { get; set; } = null!;

        /// <summary>
        /// Gets or sets the audience of the JWT.
        /// </summary>
        public string JWTAudience { get; set; } = null!;
    }
}
