// <copyright file="UserRefreshToken.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a single-use refresh token for a user.
    /// </summary>
    public class UserRefreshToken
    {
        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        [SuppressMessage(
            "Performance",
            "CA1819:Properties should not return arrays",
            Justification = "Required by EF Core.")]
        [Key]
        public byte[] TokenValue { get; set; } = null!;

        /// <summary>
        /// Gets or sets the subjected user.
        /// </summary>
        public AppUser SubjectUser { get; set; } = null!;

        /// <summary>
        /// Gets or sets the subjected user id.
        /// </summary>
        public Guid SubjectUserId { get; set; }

        /// <summary>
        /// Gets or sets the client application for which this token was issued.
        /// </summary>
        public ClientApplication ClientApplication { get; set; } = null!;

        /// <summary>
        /// Gets or sets the id of the client application for which this token was issued.
        /// </summary>
        public Guid ClientApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the IP Address where the refresh token was requested.
        /// Diagnostic purposes only.
        /// </summary>
        public string IpAddress { get; set; } = null!;

        /// <summary>
        /// Gets or sets the User Agent where the refresh token was requested.
        /// Diagnostic purposes only.
        /// </summary>
        public string UserAgent { get; set; } = null!;

        /// <summary>
        /// Gets or sets the date and time in UTC at which it was created.
        /// </summary>
        public DateTimeOffset GeneratedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time in UTC when this token expires and should
        /// no longer be considered valid.
        /// </summary>
        public DateTimeOffset ExpiresAtUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the token is no longer valid, due
        /// to either being revoked or used.
        /// </summary>
        public bool IsInvalidated { get; set; }
    }
}
