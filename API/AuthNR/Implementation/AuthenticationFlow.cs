// <copyright file="AuthenticationFlow.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.AuthNR.Implementation
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.API.AuthNR.Abstraction;
    using Fvect.Backend.API.AuthNR.Extensions;
    using Fvect.Backend.API.AuthNR.Implementation.Private;
    using Fvect.Backend.API.V1.Models;
    using Fvect.Backend.Common.Abstraction;
    using Fvect.Backend.Common.Options;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Net.Http.Headers;
    using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

    /// <summary>
    /// Impl. for <see cref="IAuthenticationFlow"/>.
    /// </summary>
    public class AuthenticationFlow : IAuthenticationFlow
    {
        private static readonly TimeSpan JwtExpiryTime = TimeSpan.FromHours(1);
        private static readonly int RefreshTokenSize = 512;
        private static readonly TimeSpan RefreshTokenExpiryTime = TimeSpan.FromDays(28);

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJwtCryptoProvider jwtCrytpoProvider;
        private readonly UserManager<AppUser> userManager;
        private readonly FvectContext dbContext;
        private readonly ISecureRandomGenerator rngGenerator;
        private readonly IMoment moment;
        private readonly IOptionsMonitor<BackendOptions> options;
        private readonly ILogger<AuthenticationFlow> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFlow"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The context accessor.</param>
        /// <param name="jwtCrytpoProvider">The crypto provider.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="dbContext">The database context.</param>
        /// <param name="rngGenerator">The random generator.</param>
        /// <param name="moment">The current moment provider.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public AuthenticationFlow(
            IHttpContextAccessor httpContextAccessor,
            IJwtCryptoProvider jwtCrytpoProvider,
            UserManager<AppUser> userManager,
            FvectContext dbContext,
            ISecureRandomGenerator rngGenerator,
            IMoment moment,
            IOptionsMonitor<BackendOptions> options,
            ILogger<AuthenticationFlow> logger)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.jwtCrytpoProvider = jwtCrytpoProvider ?? throw new ArgumentNullException(nameof(jwtCrytpoProvider));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.rngGenerator = rngGenerator ?? throw new ArgumentNullException(nameof(rngGenerator));
            this.moment = moment ?? throw new ArgumentNullException(nameof(moment));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task<AuthenticationResult?> TryAuthenticateUsingRefreshTokenAsync(
            string refreshToken, bool makeRefreshToken, Guid clientApplicationId, string? concurrencyStamp, CancellationToken ct)
            => this.Execute(
                makeRefreshToken,
                clientApplicationId,
                concurrencyStamp,
                async () =>
                {
                    var refreshTokenBinary = Convert.FromBase64String(refreshToken);

                    var refreshTokenEntity =
                        await this.dbContext.UserRefreshTokens
                            .Include(t => t.SubjectUser)
                            .Where(t => t.TokenValue == refreshTokenBinary)
                            .SingleOrDefaultAsync(ct)
                            .ConfigureAwait(false);

                    if (refreshTokenEntity is null)
                    {
                        return (false, "Unknown refresh token.", null);
                    }
                    else if (refreshTokenEntity.ClientApplicationId != clientApplicationId)
                    {
                        return (false, "Bad client id.", refreshTokenEntity.SubjectUser);
                    }
                    else if (refreshTokenEntity.ExpiresAtUtc < this.moment.UtcNow)
                    {
                        return (false, "Expired.", refreshTokenEntity.SubjectUser);
                    }
                    else if (refreshTokenEntity.IsInvalidated)
                    {
                        return (false, "Invalidated.", refreshTokenEntity.SubjectUser);
                    }
                    else if (await this.userManager.IsLockedOutAsync(refreshTokenEntity.SubjectUser).ConfigureAwait(false))
                    {
                        return (false, "User locked out.", refreshTokenEntity.SubjectUser);
                    }

                    refreshTokenEntity.IsInvalidated = true;

                    return (true, null, refreshTokenEntity.SubjectUser);
                },
                ct);

        /// <inheritdoc />
        public Task<AuthenticationResult?> TryAuthenticateUsingUserNameAndPasswordAsync(
            string userName, string password, bool makeRefreshToken, Guid clientApplicationId, string? concurrencyStamp, CancellationToken ct)
        => this.Execute(
                makeRefreshToken,
                clientApplicationId,
                concurrencyStamp,
                async () =>
                {
                    var user = await this.userManager.FindByNameAsync(userName).ConfigureAwait(false);

                    if (user is null)
                    {
                        return (false, "Unknown username.", null);
                    }

                    if (await this.userManager.IsLockedOutAsync(user).ConfigureAwait(false))
                    {
                        return (false, "User locked out.", user);
                    }

                    if (!(await this.userManager.CheckPasswordAsync(user, password).ConfigureAwait(false)))
                    {
                        return (false, "Bad password.", user);
                    }

                    return (true, null, user);
                },
                ct);

        /// <summary>
        /// Executes the shared flow. Saves pending changes on the <see cref="dbContext"/> prior to exiting.
        /// </summary>
        /// <param name="includeRefreshToken">A value indicating whether a refresh token should be generated.</param>
        /// <param name="clientApplicationId">The client application id.</param>
        /// <param name="concurrencyStamp">The concurrency stamp.</param>
        /// <param name="asyncUserRetriever">
        /// A delegate that retrieves the user, checks if the user is authenticated and
        /// returns a tuple consisting of an element that specified that authentication succeeded,
        /// a tuple consisting of an eleement that specifies an optional failure reason and an
        /// element that contains the user. The latter element is only required when the first
        /// element is true.
        /// </param>
        /// <param name="ct">A token that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// The <see cref="AuthenticationResult" />, or <c>null</c> when authentication failed.
        /// </returns>
        private async Task<AuthenticationResult?> Execute(
            bool includeRefreshToken,
            Guid clientApplicationId,
            string? concurrencyStamp,
            Func<Task<(bool succeeded, string? failureReason, AppUser? user)>> asyncUserRetriever,
            CancellationToken ct)
        {
            AuthenticationResult? result;
            var now = this.moment.UtcNow; // Retrieve time once to prevent time diff within method execution.

            // 1. Retrieve the caller info.
            var callerInfo = this.GetCallerInfo();

            // 2. Validate the client application id.
            var appIdOk = await this.ValidateApplicationIdAsync(clientApplicationId, ct).ConfigureAwait(false);
            if (!appIdOk)
            {
                this.logger.LogInformation(
                    "Aborting authentication attempt from @{callerInfo}, client application id \'{clientAppId}\' "
                       + "does not correspont to a known client application",
                    callerInfo,
                    clientApplicationId);
                return null;
            }

            // 3. Validate the user.
            var (authSucceeded, failureReason, appUser)
                = await asyncUserRetriever().ConfigureAwait(false);

            if (!authSucceeded)
            {
                // Log failure event if the user is known.
                if (!(appUser is null))
                {
                    var @event = new UserAuthenticationEvent
                    {
                        ClientApplicationId = clientApplicationId,
                        FailureReason = failureReason ?? string.Empty,
                        IncludedRefreshToken = false,
                        SubjectUserId = appUser.Id,
                        TimeStampUtc = now,
                        UserAgent = callerInfo.UserAgent,
                        IpAddress = callerInfo.IpAddress,
                    };

                    this.dbContext.UserAuthenticationEvents.Add(@event);
                }

                // Ensure no result is returned.
                result = default;
            }
            else
            {
                // 4. Generate a refresh token if required.
                string refreshToken = string.Empty;

                if (includeRefreshToken)
                {
                    refreshToken = this.CreateRefreshToken(appUser!, clientApplicationId, callerInfo, now);
                }

                // Since from this point on, a refresh token might have already been generated,
                // this is a point of no return. From this point on, the cancellation token passed to the
                // method will no longer be checked.

                // 5. Generate a JWT.
                var notBeforeTime = this.moment.UtcNow;
                var expireTime = this.moment.UtcNow.Add(JwtExpiryTime);

                var (jwtToken, tokenId) = await this.CreateJwtTokenAsync(appUser!, notBeforeTime, expireTime)
                    .ConfigureAwait(false);

                // 6. Log a successfull authentication event.
                var @event = new UserAuthenticationEvent
                {
                    ClientApplicationId = clientApplicationId,
                    FailureReason = failureReason ?? string.Empty,
                    IncludedRefreshToken = includeRefreshToken,
                    SubjectUserId = appUser!.Id,
                    TimeStampUtc = now,
                    WasSuccessful = true,
                    UserAgent = callerInfo.UserAgent,
                    IpAddress = callerInfo.IpAddress,
                    IssuedTokenId = tokenId,
                };

                this.dbContext.UserAuthenticationEvents.Add(@event);

                // 7. Create the result object.
                result = new AuthenticationResult(
                    callerInfo.UserAgent,
                    callerInfo.IpAddress,
                    clientApplicationId,
                    jwtToken,
                    refreshToken,
                    concurrencyStamp,
                    expireTime,
                    appUser.Id,
                    appUser.UserName);
            }

            // 8. Persist changes to the database.
            await this.dbContext.SaveChangesAsync(ct).ConfigureAwait(false);

            // 9. Return the result.
            return result;
        }

        private CallerInfo GetCallerInfo()
        {
            return new CallerInfo
            {
                IpAddress = this.httpContextAccessor.HttpContext.Connection.RemoteIpAddress?
                    .ToString() ?? string.Empty,
                UserAgent = this.httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent]
                    .ToString() ?? string.Empty,
            };
        }

        private Task<bool> ValidateApplicationIdAsync(Guid clientApplicationId, CancellationToken ct)
            => this.dbContext.ClientApplications
                .Where(c => c.Id == clientApplicationId)
                .AnyAsync(ct);

        private string CreateRefreshToken(
            AppUser appUser, Guid clientApplicationId, CallerInfo callerInfo, DateTimeOffset now)
        {
            // It is assumed that the user has already been authenticated.
            var randomTokenValue = this.rngGenerator.GetRandomBytes(RefreshTokenSize);

            var tokenEntity = new UserRefreshToken
            {
                TokenValue = randomTokenValue,
                ClientApplicationId = clientApplicationId,
                GeneratedAtUtc = now,
                ExpiresAtUtc = now.Add(RefreshTokenExpiryTime),
                IpAddress = callerInfo.IpAddress,
                UserAgent = callerInfo.UserAgent,
                SubjectUserId = appUser.Id,
            };

            this.dbContext.UserRefreshTokens.Add(tokenEntity);

            return Convert.ToBase64String(randomTokenValue);
        }

        private async Task<(string jwtToken, Guid tokenId)> CreateJwtTokenAsync(
            AppUser appUser, DateTimeOffset notBeforeTime, DateTimeOffset expireTime)
        {
            var credentials = this.jwtCrytpoProvider.CreateSigningCredentials();
            var tokenId = Guid.NewGuid();

            var identityUserClaims = await this.userManager.GetClaimsAsync(appUser).ConfigureAwait(false);
            var identityRoles = await this.userManager.GetRolesAsync(appUser).ConfigureAwait(false);
            var identityRoleClaims = identityRoles.Select(role => new Claim(ClaimTypes.Role, role));

            var claims = Enumerable.Empty<Claim>()
                .Append(new Claim(JwtRegisteredClaimNames.Sub, appUser.Id.ToString()))
                .Append(new Claim(JwtRegisteredClaimNames.Jti, tokenId.ToString()))
                .Append(new Claim(ClaimTypes.Name, appUser.UserName))
                .Append(new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()))
                .Concat(identityUserClaims)
                .Concat(identityRoleClaims);

            var jwt = new JwtSecurityToken(
                issuer: this.options.CurrentValue.AuthNR.JWTAuthority,
                audience: this.options.CurrentValue.AuthNR.JWTAudience,
                claims: claims,
                notBefore: notBeforeTime.UtcDateTime,
                expires: expireTime.UtcDateTime,
                credentials);

            var jwtString = new JwtSecurityTokenHandler().WriteToken(jwt);

            return (jwtString, tokenId);
        }
    }
}
