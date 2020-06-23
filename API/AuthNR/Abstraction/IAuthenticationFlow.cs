// <copyright file="IAuthenticationFlow.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.AuthNR.Abstraction
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.API.V1.Models;

    /// <summary>
    /// Specifies an object that can execute the authentication flow of the application.
    /// </summary>
    /// <remarks>
    /// It is a design choice to implement this in the API layer instead of the business layer.
    /// </remarks>
    public interface IAuthenticationFlow
    {
        /// <summary>
        /// Tries to authenticate using the supplied values.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="makeRefreshToken">A value indicating whether a new refresh token must be generated.</param>
        /// <param name="clientApplicationId">The client application id.</param>
        /// <param name="concurrencyStamp">The concurrency stamp.</param>
        /// <param name="ct">A token that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// If succeeded: an <see cref="AuthenticationResult"/>. Else <c>null</c>.
        /// </returns>
        Task<AuthenticationResult?> TryAuthenticateUsingUserNameAndPasswordAsync(
            string userName,
            string password,
            bool makeRefreshToken,
            Guid clientApplicationId,
            string? concurrencyStamp,
            CancellationToken ct);

        /// <summary>
        /// Tries to authenticate using the supplied values.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="makeRefreshToken">A value indicating whether a new refresh token must be generated.</param>
        /// <param name="clientApplicationId">The client application id.</param>
        /// <param name="concurrencyStamp">The concurrency stamp.</param>
        /// <param name="ct">A token that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// If succeeded: an <see cref="AuthenticationResult"/>. Else <c>null</c>.
        /// </returns>
        Task<AuthenticationResult?> TryAuthenticateUsingRefreshTokenAsync(
            string refreshToken,
            bool makeRefreshToken,
            Guid clientApplicationId,
            string? concurrencyStamp,
            CancellationToken ct);
    }
}
