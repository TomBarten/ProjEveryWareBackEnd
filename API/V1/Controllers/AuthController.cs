// <copyright file="AuthController.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.API.AuthNR.Abstraction;
    using Fvect.Backend.API.V1.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Represents a controller for AuthN/AuhtR services.
    /// </summary>
    /// <remarks>
    /// Models used by this controller are validated using fluent validation validators.
    /// </remarks>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationFlow authNFlow;
        private readonly IAuthManager authManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authNFlow">The authentication flow.</param>
        /// <param name="authManager">The auth manager.</param>
        public AuthController(
            IAuthenticationFlow authNFlow,
            IAuthManager authManager)
        {
            this.authNFlow = authNFlow;
            this.authManager = authManager;
        }

        /// <summary>
        /// Authenticates using username and password.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="ct">A token that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// The authentication response, if successful.
        /// </returns>
        /// <response code="200">When the request was executed sucessfully.</response>
        /// <response code="400">When the request is not valid.</response>
        /// <response code="401">When authentication failed.</response>
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<ActionResult<AuthenticationResult>> Authenticate(
            [FromBody] AuthenticationRequest request,
            CancellationToken ct)
        {
            var result = await this.authNFlow.TryAuthenticateUsingUserNameAndPasswordAsync(
                request.UserName,
                request.Password,
                request.IncludeRefreshToken,
                request.ClientApplicationId,
                request.ConcurrencyStamp,
                ct)
                .ConfigureAwait(true);

            if (result is null)
            {
                return this.Unauthorized();
            }

            return result;
        }

        /// <summary>
        /// Refreshes auhtentication using a refresh token. If successful, the token has been invalidated.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="ct">A token that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// The authentication response, if successful.
        /// </returns>
        /// <response code="200">When the request was executed sucessfully.</response>
        /// <response code="400">When the request is not valid.</response>
        /// <response code="401">When authentication failed.</response>
        [AllowAnonymous]
        [HttpPost("Refresh")]
        public async Task<ActionResult<AuthenticationResult>> Refresh(
            [FromBody] RefreshRequest request,
            CancellationToken ct)
        {
            var result = await this.authNFlow.TryAuthenticateUsingRefreshTokenAsync(
                request.RefreshToken,
                request.IncludeNewRefreshToken,
                request.ClientApplicationId,
                request.ConcurrencyStamp,
                ct)
                .ConfigureAwait(true);

            if (result is null)
            {
                return this.Unauthorized();
            }

            return result;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An empty body when no errors occurred.</returns>
        /// <response code="204">When the request was executed successfully.</response>
        /// <response code="400">When the request is not valid.</response>
        /// <response code="409">When an error occured when creating the user.</response>
        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(
            [FromBody] CreateUserRequest request)
        {
            var result = await this.authManager.CreateUserAsync(
                request.UserName,
                request.Password)
                .ConfigureAwait(false);

            if (!(result is null))
            {
                return this.Conflict(result);
            }

            return this.NoContent();
        }

        /// <summary>
        /// Revokes all refresh tokens for the current user, if any.
        /// </summary>
        /// <returns>
        /// An empty body.
        /// </returns>
        /// <response code="204">When the operation completed succesfully.</response>
        /// <response code="400">When the request is not valid.</response>
        /// <response code="401">When the user is not authenticated.</response>
        [Authorize]
        [HttpPost("InvalidateRefreshTokens")]
        public Task InvalidateRefreshTokens() => this.authManager.InvalidateRefreshTokensAsync(this.User);

        /// <summary>
        /// Revokes the given refresh token.
        /// </summary>
        /// <param name="tokenInvalidationRequest">The request model.</param>
        /// <response code="204">When the operation completed succesfully.</response>
        /// <response code="400">When the request is not valid.</response>
        /// <response code="401">When the user is not authenticated.</response>
        /// <response code="404">When the supplied token was not found in a valid state for the current user.</response>
        /// <returns>An empty response.</returns>
        [Authorize]
        [HttpPost("InvalidateRefreshToken")]
        public async Task<ActionResult> InvalidateRefreshToken(
            [FromBody] RefreshTokenInvalidationRequest tokenInvalidationRequest)
        {
            if (await
                this.authManager.InvalidateRefreshTokenAsync(
                    this.User, tokenInvalidationRequest.Token)
                .ConfigureAwait(true))
            {
                return this.NoContent();
            }

            return this.NotFound();
        }
    }
}
