// <copyright file="RefreshTokenInvalidationRequest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Models
{
    /// <summary>
    /// Represents a request to invalidate a refresh token.
    /// </summary>
    public class RefreshTokenInvalidationRequest
    {
        /// <summary>
        /// Gets or sets the base64 encoded refresh token that is to be invalidated.
        /// </summary>
        public string Token { get; set; } = null!; // Enforced by validation.
    }
}
