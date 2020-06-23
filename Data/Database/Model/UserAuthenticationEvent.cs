// <copyright file="UserAuthenticationEvent.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents an authentication event of a user. For invalid usernames, no event is logged.
    /// </summary>
    public class UserAuthenticationEvent
    {
        /// <summary>
        /// Gets or sets the unique identifier of this event.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the timesamp of this event in UTC time.
        /// </summary>
        public DateTimeOffset TimeStampUtc { get; set; }

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
        /// Gets or sets the id of the issued JWT token, if any.
        /// </summary>
        public Guid? IssuedTokenId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attempt was succesful
        /// and thus a bearer token was provided.
        /// </summary>
        public bool WasSuccessful { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a refresh token was provided
        /// in the result.
        /// </summary>
        public bool IncludedRefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the reason that authentication has failed.
        /// </summary>
        public string? FailureReason { get; set; } = null!;

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
    }
}
