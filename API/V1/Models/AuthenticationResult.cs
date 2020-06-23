// <copyright file="AuthenticationResult.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Models
{
    using System;

    /// <summary>
    /// Represents the result of a successful authentication attempt.
    /// </summary>
    public class AuthenticationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationResult"/> class.
        /// </summary>
        /// <param name="userAgent">The value for <see cref="UserAgent"/>.</param>
        /// <param name="ipAddress">The value for <see cref="IpAddress"/>.</param>
        /// <param name="clientApplicationId">The value for <see cref="ClientApplicationId"/>.</param>
        /// <param name="bearerToken">The value for <see cref="BearerToken"/>.</param>
        /// <param name="refreshToken">The value for <see cref="RefreshToken"/>.</param>
        /// <param name="concurrencyStamp">The value for <see cref="ConcurrencyStamp"/>.</param>
        /// <param name="bearerExpirationUtc">The value for <see cref="BearerExpirationUtc"/>.</param>
        /// <param name="userId">The value for <see cref="UserId"/>.</param>
        /// <param name="userUserName">The value for <see cref="UserUserName"/>.</param>
        public AuthenticationResult(
            string userAgent,
            string ipAddress,
            Guid clientApplicationId,
            string bearerToken,
            string? refreshToken,
            string? concurrencyStamp,
            DateTimeOffset bearerExpirationUtc,
            Guid userId,
            string userUserName)
        {
            this.UserAgent = userAgent;
            this.IpAddress = ipAddress;
            this.ClientApplicationId = clientApplicationId;
            this.BearerToken = bearerToken;
            this.RefreshToken = refreshToken;
            this.ConcurrencyStamp = concurrencyStamp;
            this.BearerExpirationUtc = bearerExpirationUtc;
            this.UserId = userId;
            this.UserUserName = userUserName;
        }

        /// <summary>
        /// Gets or sets the user agent of the user, for diagnostic puproses.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the user, for diagnostic purposes.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the client application, for diagnostic purposes.
        /// </summary>
        public Guid ClientApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the short-lived Bearer token that is to be used for authorization.
        /// </summary>
        public string BearerToken { get; set; }

        /// <summary>
        /// Gets or sets the long-lived single-use refresh token that can be used to request a new
        /// bearer token if the original one has expired or the roles of the user have changed.
        /// Only available if originally requested.
        /// Clients should treat this value as sensitive and take appropriate measures to protect it.
        /// Should this value leak, please contact the API administrator to ensure all tokens granted
        /// with your client id are revoked.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the concurreny stamp passed into the request, if any.
        /// </summary>
        public string? ConcurrencyStamp { get; set; }

        /// <summary>
        /// Gets or sets the expiration time of the bearer token in UTC.
        /// </summary>
        public DateTimeOffset BearerExpirationUtc { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user that this result belongs to.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the user name of the user that this result belongs to.
        /// </summary>
        public string UserUserName { get; set; }
    }
}
