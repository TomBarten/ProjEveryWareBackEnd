// <copyright file="CreateUserRequest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Models
{
    /// <summary>
    /// Represents a request used to create a user.
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// Gets or sets the desired UserName.
        /// </summary>
        public string UserName { get; set; } = null!; // Enforced by validation.

        /// <summary>
        /// Gets or sets the desired password.
        /// </summary>
        public string Password { get; set; } = null!; // Enforced by validation.
    }
}
