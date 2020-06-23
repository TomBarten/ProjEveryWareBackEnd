// <copyright file="AuthManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.AuthNR.Implementation
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Fvect.Backend.API.AuthNR.Abstraction;
    using Fvect.Backend.Common.Abstraction;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Impl. for <see cref="IAuthManager"/>.
    /// </summary>
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<AppUser> userManager;
        private readonly FvectContext dbContext;
        private readonly IMoment moment;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthManager"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="dbContext">The database context.</param>
        /// <param name="moment">The moment.</param>
        public AuthManager(
            UserManager<AppUser> userManager,
            FvectContext dbContext,
            IMoment moment)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.moment = moment;
        }

        /// <inheritdoc />
        public async Task<string?> CreateUserAsync(string userName, string password)
        {
            var user = new AppUser
            {
                UserName = userName,
            };

            var result = await this.userManager
                .CreateAsync(user, password)
                .ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<bool> InvalidateRefreshTokenAsync(ClaimsPrincipal user, string refreshToken)
        {
            var userId = Guid.Parse(this.userManager.GetUserId(user));
            var tokenBinary = Convert.FromBase64String(refreshToken);

            var token = await this.dbContext.UserRefreshTokens
                .Where(
                    t => t.TokenValue == tokenBinary
                    && t.SubjectUserId == userId
                    && !t.IsInvalidated
                    && t.ExpiresAtUtc > this.moment.UtcNow)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (!(token is null))
            {
                token.IsInvalidated = true;
                await this.dbContext.SaveChangesAsync().ConfigureAwait(false);

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public async Task InvalidateRefreshTokensAsync(ClaimsPrincipal user)
        {
            var userId = Guid.Parse(this.userManager.GetUserId(user));

            await foreach (var token in this.dbContext.UserRefreshTokens
                .Where(
                    t => t.SubjectUserId == userId && !t.IsInvalidated && t.ExpiresAtUtc > this.moment.UtcNow)
                .AsAsyncEnumerable()
                .ConfigureAwait(false))
            {
                token.IsInvalidated = true;
            }

            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
