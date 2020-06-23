// <copyright file="AuthenticationRequest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Models
{
    using System;

    /// <summary>
    /// Represents a request for a bearer token using a username and a password.
    /// </summary>
    public class AuthenticationRequest
    {
        /// <summary>
        /// Gets or sets the identifier of the client application.
        /// </summary>
        public Guid ClientApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether a refresh token should
        /// be generated. Defaults to <c>false</c>.
        /// </summary>
        public bool IncludeRefreshToken { get; set; } = false;

        /// <summary>
        /// Gets or sets a concurrency stamp that will be returned in the resposne, if successful.
        /// </summary>
        public string? ConcurrencyStamp { get; set; }
    }
}
