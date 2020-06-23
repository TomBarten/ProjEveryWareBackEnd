// <copyright file="RefreshRequest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Models
{
    using System;

    /// <summary>
    /// Represents a request for a new bearer token using a refresh token.
    /// </summary>
    public class RefreshRequest
    {
        /// <summary>
        /// Gets or sets the identifier of the client application.
        /// </summary>
        public Guid ClientApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether a new refresh token should
        /// be generated. Defaults to <c>false</c>.
        /// </summary>
        public bool IncludeNewRefreshToken { get; set; } = false;

        /// <summary>
        /// Gets or sets a concurrency stamp that will be returned in the response, if successful.
        /// </summary>
        public string? ConcurrencyStamp { get; set; }
    }
}
