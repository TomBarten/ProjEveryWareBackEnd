// <copyright file="IAuthManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.AuthNR.Abstraction
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    /// <summary>
    /// Specifies an object that can manage AuthN/AuthR services.
    /// </summary>
    /// <remarks>
    /// Cancellation of operations is not supported due to security requirements.
    /// </remarks>
    public interface IAuthManager
    {
        /// <summary>
        /// Asynchronously creates a new user.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// A possible error message when the operation did not execute gracefully.
        /// </returns>
        Task<string?> CreateUserAsync(string userName, string password);

        /// <summary>
        /// Invalidates all refresh tokens for the given <paramref name="user"/>.
        /// </summary>
        /// <param name="user">
        /// The user for which the refresh tokens must be invalidated.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task InvalidateRefreshTokensAsync(ClaimsPrincipal user);

        /// <summary>
        /// Invalidates the given <paramref name="refreshToken"/> for the given <paramref name="user"/>.
        /// </summary>
        /// <param name="user">
        /// The user for which the refresh tokens must be invalidated.
        /// </param>
        /// <param name="refreshToken">
        /// The base64 encoded refresh token.
        /// </param>
        /// <returns>
        /// A boolean indicating if the <paramref name="refreshToken"/> was found in a valid state.
        /// </returns>
        Task<bool> InvalidateRefreshTokenAsync(ClaimsPrincipal user, string refreshToken);
    }
}
