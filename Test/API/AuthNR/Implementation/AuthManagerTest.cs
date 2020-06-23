// <copyright file="AuthManagerTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.AuthNR.Implementation
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.API.AuthNR.Implementation;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.EntityFrameworkCore;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Contains tests methods for the <see cref="AuthManager"/>.
    /// </summary>
    [SuppressMessage(
        "Security",
        "SCS0005:Weak random generator",
        Justification = "Only used for testing purposes.")]
    public class AuthManagerTest
    {
        private readonly ITestOutputHelper outputHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthManagerTest"/> class.
        /// </summary>
        /// <param name="outputHelper">The output helper.</param>
        public AuthManagerTest(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        /// <summary>
        /// Tests the happy flow of creating a user.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateUserAsyncHappyFlow()
        {
            var momentMock = MomentTestHelper.CreateMomentMock();
            using var testDb = new TestDatabase(this.outputHelper);
            using var identityInstance = IdentityTestHelper.CreateInstance(testDb, this.outputHelper);
            using var testDbContext = await testDb.CreateContextAsync().ConfigureAwait(true);

            var manager = new AuthManager(
                identityInstance.UserManager,
                testDbContext,
                momentMock.Object);

            var userName = "Someone";
            var password = "Someone1";

            var result = await manager.CreateUserAsync(userName, password).ConfigureAwait(true);

            result.Should().BeNull(because: "no errors should have occurred");

            var user = await identityInstance.UserManager.FindByNameAsync(userName).ConfigureAwait(true);

            user.UserName.Should().Be(userName, because: "this was the user name of the created user");

            var userEntity = await testDbContext.Users.FindAsync(user.Id).ConfigureAwait(true);
            userEntity.Should().NotBeNull(because: "the user should have been inserted into the database");
        }

        /// <summary>
        /// Tests creating a user with an empty password.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateUserEmptyPassword()
        {
            var momentMock = MomentTestHelper.CreateMomentMock();
            using var testDb = new TestDatabase(this.outputHelper);
            using var identityInstance = IdentityTestHelper.CreateInstance(testDb, this.outputHelper);
            using var testDbContext = await testDb.CreateContextAsync().ConfigureAwait(true);

            var manager = new AuthManager(
                identityInstance.UserManager,
                testDbContext,
                momentMock.Object);

            var userName = "Someone";
            var password = string.Empty;

            var result = await manager.CreateUserAsync(userName, password).ConfigureAwait(true);

            result.Should().NotBeEmpty(because: "an error should have occured");

            var user = await identityInstance.UserManager.FindByNameAsync(userName).ConfigureAwait(true);

            user.Should().BeNull(because: "the user should have never been created");
        }

        /// <summary>
        /// Tests the happy flow of invalidating a refresh token.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        [Fact]
        public async Task InvalidateRefreshTokenAsyncHappyFlow()
        {
            var tokenValue = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var tokenValueStr = Convert.ToBase64String(tokenValue);

            var momentMock = MomentTestHelper.CreateMomentMock();
            using var testDb = new TestDatabase(this.outputHelper);
            using var identityInstance = IdentityTestHelper.CreateInstance(testDb, this.outputHelper);
            using var testDbContext = await testDb.CreateContextAsync().ConfigureAwait(true);

            var manager = new AuthManager(
                identityInstance.UserManager,
                testDbContext,
                momentMock.Object);

            var user = await identityInstance.CreateUserAsync("someone").ConfigureAwait(true);
            var claimsPrincipal = IdentityTestHelper.CreateClaimsPrincipalForUserId(user.Id);

            var tokenEntity = new UserRefreshToken
            {
                TokenValue = tokenValue,
                IpAddress = "1.2.3",
                UserAgent = "chrome",
                IsInvalidated = false,
                SubjectUserId = user.Id,
                GeneratedAtUtc = momentMock.Object.UtcNow,
                ExpiresAtUtc = momentMock.Object.UtcNow.AddDays(1),
                ClientApplicationId = identityInstance.ClientId,
            };

            testDbContext.UserRefreshTokens.Add(tokenEntity);
            await testDbContext.SaveChangesAsync().ConfigureAwait(true);

            var result = await manager.InvalidateRefreshTokenAsync(
                claimsPrincipal,
                tokenValueStr).ConfigureAwait(true);

            result.Should().BeTrue(because: "the token should have been found and invalidated");

            tokenEntity = await testDbContext.UserRefreshTokens
                .FindAsync(tokenValue)
                .ConfigureAwait(true);

            tokenEntity.IsInvalidated.Should().BeTrue(
                because: "the invalidation should have been persisted to the database");
        }

        /// <summary>
        /// Tests invalidating a refresh token that does not belong to the user.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        [Fact]
        public async Task InvalidateRefreshTokenDoesNotBelongToUser()
        {
            var tokenValue = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var tokenValueStr = Convert.ToBase64String(tokenValue);

            var momentMock = MomentTestHelper.CreateMomentMock();
            using var testDb = new TestDatabase(this.outputHelper);
            using var identityInstance = IdentityTestHelper.CreateInstance(testDb, this.outputHelper);
            using var testDbContext = await testDb.CreateContextAsync().ConfigureAwait(true);

            var manager = new AuthManager(
                identityInstance.UserManager,
                testDbContext,
                momentMock.Object);

            var user = await identityInstance.CreateUserAsync("someone").ConfigureAwait(true);
            var user2 = await identityInstance.CreateUserAsync("someone1").ConfigureAwait(true);
            var claimsPrincipal = IdentityTestHelper.CreateClaimsPrincipalForUserId(user.Id);

            var tokenEntity = new UserRefreshToken
            {
                TokenValue = tokenValue,
                IpAddress = "1.2.3",
                UserAgent = "chrome",
                IsInvalidated = false,
                SubjectUserId = user2.Id,
                GeneratedAtUtc = momentMock.Object.UtcNow,
                ExpiresAtUtc = momentMock.Object.UtcNow.AddDays(1),
                ClientApplicationId = identityInstance.ClientId,
            };

            testDbContext.UserRefreshTokens.Add(tokenEntity);
            await testDbContext.SaveChangesAsync().ConfigureAwait(true);

            var result = await manager.InvalidateRefreshTokenAsync(
                claimsPrincipal,
                tokenValueStr).ConfigureAwait(true);

            result.Should().BeFalse(because: "the token should not have been found and invalidated");

            tokenEntity = await testDbContext.UserRefreshTokens
                .FindAsync(tokenValue)
                .ConfigureAwait(true);

            tokenEntity.IsInvalidated.Should().BeFalse(
                because: "the token was not invalidated");
        }

        /// <summary>
        /// Tests invalidating a refresh token that is already expired.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        [Fact]
        public async Task InvalidateRefreshTokenAlreadyExpired()
        {
            var tokenValue = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var tokenValueStr = Convert.ToBase64String(tokenValue);

            var momentMock = MomentTestHelper.CreateMomentMock();
            using var testDb = new TestDatabase(this.outputHelper);
            using var identityInstance = IdentityTestHelper.CreateInstance(testDb, this.outputHelper);
            using var testDbContext = await testDb.CreateContextAsync().ConfigureAwait(true);

            var manager = new AuthManager(
                identityInstance.UserManager,
                testDbContext,
                momentMock.Object);

            var user = await identityInstance.CreateUserAsync("someone").ConfigureAwait(true);
            var claimsPrincipal = IdentityTestHelper.CreateClaimsPrincipalForUserId(user.Id);

            var tokenEntity = new UserRefreshToken
            {
                TokenValue = tokenValue,
                IpAddress = "1.2.3",
                UserAgent = "chrome",
                IsInvalidated = false,
                SubjectUserId = user.Id,
                GeneratedAtUtc = momentMock.Object.UtcNow,
                ExpiresAtUtc = momentMock.Object.UtcNow.AddDays(-1),
                ClientApplicationId = identityInstance.ClientId,
            };

            testDbContext.UserRefreshTokens.Add(tokenEntity);
            await testDbContext.SaveChangesAsync().ConfigureAwait(true);

            var result = await manager.InvalidateRefreshTokenAsync(
                claimsPrincipal,
                tokenValueStr).ConfigureAwait(true);

            result.Should().BeFalse(because: "the token should not have been found and invalidated");

            tokenEntity = await testDbContext.UserRefreshTokens
                .FindAsync(tokenValue)
                .ConfigureAwait(true);

            tokenEntity.IsInvalidated.Should().BeFalse(
                because: "the token was not invalidated");
        }

        /// <summary>
        /// Tests the happy flow of revoking all refresh tokens of a user,
        /// asserting that tokens of other users are not affected.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        [Fact]
        public async Task InvalidateRefreshTokensHappyFlow()
        {
            var rnd = new Random();
            var tokenValues = Enumerable.Range(0, 3).Select(v =>
            {
                var arr = new byte[8];
                rnd.NextBytes(arr);
                return arr;
            })
                .ToImmutableList();

            var momentMock = MomentTestHelper.CreateMomentMock();
            using var testDb = new TestDatabase(this.outputHelper);
            using var identityInstance = IdentityTestHelper.CreateInstance(testDb, this.outputHelper);
            using var testDbContext = await testDb.CreateContextAsync().ConfigureAwait(true);

            var manager = new AuthManager(
                identityInstance.UserManager,
                testDbContext,
                momentMock.Object);

            var user = await identityInstance.CreateUserAsync("someone").ConfigureAwait(true);
            var user2 = await identityInstance.CreateUserAsync("someone1").ConfigureAwait(true);
            var claimsPrincipal = IdentityTestHelper.CreateClaimsPrincipalForUserId(user.Id);

            testDbContext.UserRefreshTokens.AddRange(tokenValues.Select((t, i) => new UserRefreshToken
            {
                TokenValue = t,
                IpAddress = "1.2.3",
                UserAgent = "chrome",
                IsInvalidated = false,
                SubjectUserId = i < 2 ? user.Id : user2.Id,
                GeneratedAtUtc = momentMock.Object.UtcNow,
                ExpiresAtUtc = momentMock.Object.UtcNow.AddDays(1),
                ClientApplicationId = identityInstance.ClientId,
            }));
            await testDbContext.SaveChangesAsync().ConfigureAwait(true);

            await manager.InvalidateRefreshTokensAsync(
                claimsPrincipal).ConfigureAwait(true);

            (await testDbContext
                .UserRefreshTokens
                .Where(x => x.SubjectUserId == user.Id)
                .Select(x => x.IsInvalidated)
                .AllAsync(x => x).ConfigureAwait(true))
                .Should()
                .BeTrue(because: "the tokens should have been invalidated");

            (await testDbContext
                .UserRefreshTokens
                .Where(x => x.SubjectUserId == user2.Id)
                .Select(x => x.IsInvalidated)
                .AllAsync(x => !x).ConfigureAwait(true))
                .Should()
                .BeTrue(because: "the tokens should not have been invalidated");
        }
    }
}
