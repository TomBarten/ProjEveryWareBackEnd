// <copyright file="AuthenticationFlowTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.AuthNR.Implementation
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.API.AuthNR.Abstraction;
    using Fvect.Backend.API.AuthNR.Implementation;
    using Fvect.Backend.Common.Abstraction;
    using Fvect.Backend.Common.Implementation;
    using Fvect.Backend.Common.Options;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Moq;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Contains tests for the <see cref="AuthenticationFlowTest"/> class.
    /// </summary>
    public sealed class AuthenticationFlowTest : IDisposable
    {
        private static readonly SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                new string(
                    Enumerable.Range('a', 16).Select(a => Convert.ToChar(a)).ToArray())));

        private static readonly string SigningAlgorithm = SecurityAlgorithms.HmacSha512Signature;

        private static readonly string UserName = "AuthUser";
        private static readonly string Password = IdentityTestHelper.Instance.DefaultPassword;
        private static readonly string InvalidPassword = "p";
        private static readonly string FlowConcurrencyStamp = "concurrency";
        private static readonly string TestRole = "TestRole";

        private readonly ITestOutputHelper outputHelper;
        private readonly TestDatabase testDatabase;
        private readonly FvectContext dbContext;
        private readonly IdentityTestHelper.Instance identityInstance;
        private readonly HttpContext usedHttpContext;
        private readonly Mock<IHttpContextAccessor> httpConctextAccessorMock;
        private readonly Mock<IJwtCryptoProvider> jwtCrypoMock;
        private readonly Mock<IMoment> momentMock;
        private readonly Mock<IOptionsMonitor<BackendOptions>> optionsMonitorMock;
        private readonly SecureRandomGenerator randomGenerator;
        private readonly ILoggerFactory loggerFactory;
        private readonly AuthenticationFlow authenticationFlow;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFlowTest"/> class.
        /// </summary>
        /// <param name="outputHelper">The test output helper.</param>
        public AuthenticationFlowTest(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
            this.testDatabase = new TestDatabase(outputHelper);
            this.dbContext = this.testDatabase.CreateContext();
            this.identityInstance = IdentityTestHelper.CreateInstance(this.testDatabase, this.outputHelper);
            this.usedHttpContext = new DefaultHttpContext();
            this.httpConctextAccessorMock = new Mock<IHttpContextAccessor>();
            this.jwtCrypoMock = new Mock<IJwtCryptoProvider>();
            this.momentMock = MomentTestHelper.CreateMomentMock();
            this.optionsMonitorMock = OptionsTestHelper.CreateBackendOptionsMock();
            this.randomGenerator = new SecureRandomGenerator();
            this.loggerFactory = LoggingTestHelper.CreateLoggerFactory(this.outputHelper);

            this.httpConctextAccessorMock.SetupGet(m => m.HttpContext).Returns(this.usedHttpContext);

            this.jwtCrypoMock.SetupGet(c => c.Algorithm).Returns(SigningAlgorithm);
            this.jwtCrypoMock.SetupGet(c => c.SecurityKey).Returns(SigningKey);

            this.authenticationFlow = new AuthenticationFlow(
                this.httpConctextAccessorMock.Object,
                this.jwtCrypoMock.Object,
                this.identityInstance.UserManager,
                this.dbContext,
                this.randomGenerator,
                this.momentMock.Object,
                this.optionsMonitorMock.Object,
                this.loggerFactory.CreateLogger<AuthenticationFlow>());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.randomGenerator.Dispose();
            this.dbContext.Dispose();
            this.identityInstance.Dispose();
            this.loggerFactory.Dispose();
            this.testDatabase.Dispose();
        }

        /// <summary>
        /// Tests the happy flow of obtaining a token using a username and password
        /// before obtaining a new token with a refresh token and a role.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        [Fact]
        public async Task UserNamePasswordAndRefreshTokenHappyFlowWithRole()
        {
            await this.identityInstance.RoleManager.CreateAsync(new AppRole
            {
                Name = TestRole,
            }).ConfigureAwait(true);

            var user = await this.identityInstance.CreateUserAsync(UserName).ConfigureAwait(true);
            await this.identityInstance.UserManager.AddToRoleAsync(user, TestRole).ConfigureAwait(true);

            var authenticationResult =
                await this.authenticationFlow.TryAuthenticateUsingUserNameAndPasswordAsync(
                    UserName,
                    Password,
                    true,
                    this.identityInstance.ClientId,
                    FlowConcurrencyStamp,
                    CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().NotBeNull(because: "the sign-in attempt should have succeeded");
            authenticationResult!.ClientApplicationId.Should().Be(
                this.identityInstance.ClientId,
                because: "the client identifier should be echoed");
            authenticationResult.ConcurrencyStamp.Should().Be(
                FlowConcurrencyStamp,
                because: "the concurrency stamp should be echoed");
            authenticationResult.RefreshToken.Should().NotBeEmpty(because: "a refresh token was requested");

            this.ValidateBearer(authenticationResult.BearerToken, user.Id);

            var refreshTokenBinary = Convert.FromBase64String(authenticationResult.RefreshToken!);

            var refreshTokenEntity = await this.dbContext.UserRefreshTokens.FindAsync(refreshTokenBinary)
                .ConfigureAwait(true);

            refreshTokenEntity.Should().NotBeNull(because: "the refresh token must be persisted");
            refreshTokenEntity.ExpiresAtUtc.Should().Be(this.momentMock.Object.UtcNow.AddDays(28));
            refreshTokenEntity.SubjectUserId.Should().Be(user.Id, because: "the refresh token must be linked to the user");
            refreshTokenEntity.IsInvalidated.Should().BeFalse(because: "the refresh token is not used or invalidated");

            authenticationResult =
                await this.authenticationFlow.TryAuthenticateUsingRefreshTokenAsync(
                    authenticationResult.RefreshToken!,
                    false,
                    this.identityInstance.ClientId,
                    FlowConcurrencyStamp,
                    CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().NotBeNull(because: "the sign-in attempt should have succeeded");
            authenticationResult!.ClientApplicationId.Should().Be(
                this.identityInstance.ClientId,
                because: "the client identifier should be echoed");
            authenticationResult.ConcurrencyStamp.Should().Be(
                FlowConcurrencyStamp,
                because: "the concurrency stamp should be echoed");
            authenticationResult.RefreshToken.Should().BeEmpty(because: "no refresh token was requested");

            this.ValidateBearer(authenticationResult.BearerToken, user.Id, TestRole);

            refreshTokenEntity = await this.dbContext.UserRefreshTokens.FindAsync(refreshTokenBinary)
                 .ConfigureAwait(true);

            refreshTokenEntity.IsInvalidated.Should().BeTrue(because: "the refresh token was used and therefore invalidated");
        }

        /// <summary>
        /// Tests authenticating with a correct user name and password but the account is locked out.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task CorrectPasswordLockedOut()
        {
            var user = await this.identityInstance.CreateUserAsync(UserName).ConfigureAwait(true);

            await this.SetUserLockedOut(user).ConfigureAwait(true);

            var authenticationResult =
                await this.authenticationFlow.TryAuthenticateUsingUserNameAndPasswordAsync(
                    UserName,
                    Password,
                    true,
                    this.identityInstance.ClientId,
                    FlowConcurrencyStamp,
                    CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().BeNull(because: "the user is locked out");
        }

        /// <summary>
        /// Tests authenticating with a wrong password.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task WrongPassword()
        {
            await this.identityInstance.CreateUserAsync(UserName).ConfigureAwait(true);

            var authenticationResult =
                await this.authenticationFlow.TryAuthenticateUsingUserNameAndPasswordAsync(
                    UserName,
                    InvalidPassword,
                    true,
                    this.identityInstance.ClientId,
                    FlowConcurrencyStamp,
                    CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().BeNull(because: "the password was not correct");
        }

        /// <summary>
        /// Tests authenticating with an unknown username.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task UnknownUserName()
        {
            await this.identityInstance.CreateUserAsync(UserName).ConfigureAwait(true);

            var authenticationResult =
                await this.authenticationFlow.TryAuthenticateUsingUserNameAndPasswordAsync(
                    $"{UserName} ", // Most annoying thing when trying to log in is a space after your username
                    InvalidPassword,
                    true,
                    this.identityInstance.ClientId,
                    FlowConcurrencyStamp,
                    CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().BeNull(because: "the user name was not known");
        }

        /// <summary>
        /// Test authentication with a correct refresh token but the account is locked out.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task CorrectTokenLockedOut()
        {
            var user = await this.identityInstance.CreateUserAsync(UserName).ConfigureAwait(true);

            await this.SetUserLockedOut(user).ConfigureAwait(true);

            var (_, tokenStr) = await this.CreateRefreshTokenForUser(user.Id).ConfigureAwait(true);

            var authenticationResult = await this.authenticationFlow.TryAuthenticateUsingRefreshTokenAsync(
                tokenStr,
                false,
                this.identityInstance.ClientId,
                string.Empty,
                CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().BeNull(because: "the user is locked out");
        }

        /// <summary>
        /// Test authenticating with an invalid refresh token.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task InvalidatedToken()
        {
            var user = await this.identityInstance.CreateUserAsync(UserName).ConfigureAwait(true);

            var (_, tokenStr) = await this.CreateRefreshTokenForUser(user.Id, isInvalidated: true).ConfigureAwait(true);

            var authenticationResult = await this.authenticationFlow.TryAuthenticateUsingRefreshTokenAsync(
                tokenStr,
                false,
                this.identityInstance.ClientId,
                string.Empty,
                CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().BeNull(because: "the token was not valid");
        }

        /// <summary>
        /// Test authenticating with an expired refresh token.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task ExpiredToken()
        {
            var user = await this.identityInstance.CreateUserAsync(UserName).ConfigureAwait(true);

            var (_, tokenStr) = await this.CreateRefreshTokenForUser(user.Id, expireAt: this.momentMock.Object.UtcNow.AddDays(-1))
                .ConfigureAwait(true);

            var authenticationResult = await this.authenticationFlow.TryAuthenticateUsingRefreshTokenAsync(
                tokenStr,
                false,
                this.identityInstance.ClientId,
                string.Empty,
                CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().BeNull(because: "the token was not valid");
        }

        /// <summary>
        /// Test authenticating with an unknown refresh token.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task UnknownToken()
        {
            await this.identityInstance.CreateUserAsync(UserName).ConfigureAwait(true);

            var tokenStr = Convert.ToBase64String(this.randomGenerator.GetRandomBytes(64));

            var authenticationResult = await this.authenticationFlow.TryAuthenticateUsingRefreshTokenAsync(
                tokenStr,
                false,
                this.identityInstance.ClientId,
                string.Empty,
                CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().BeNull(because: "the token was not not known");
        }

        /// <summary>
        /// Test authenticating with a bad client id and a refresh token (different client id).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task BadClientId()
        {
            var someClientEntity = new ClientApplication
            {
                Name = "Postman",
            };

            this.dbContext.ClientApplications.Add(someClientEntity);
            await this.dbContext.SaveChangesAsync().ConfigureAwait(true);

            var user = await this.identityInstance.CreateUserAsync(UserName).ConfigureAwait(true);

            var (_, tokenStr) = await this.CreateRefreshTokenForUser(user.Id, appId: someClientEntity.Id)
                .ConfigureAwait(true);

            var authenticationResult = await this.authenticationFlow.TryAuthenticateUsingRefreshTokenAsync(
                tokenStr,
                false,
                this.identityInstance.ClientId,
                string.Empty,
                CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().BeNull(because: "the client id was not equivalent to the id saved with the token");
        }

        /// <summary>
        /// Test authenticating with an unknown client id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task UnknownClientId()
        {
            var user = await this.identityInstance.CreateUserAsync(UserName).ConfigureAwait(true);

            var (_, tokenStr) = await this.CreateRefreshTokenForUser(user.Id)
                .ConfigureAwait(true);

            var authenticationResult = await this.authenticationFlow.TryAuthenticateUsingRefreshTokenAsync(
                tokenStr,
                false,
                Guid.NewGuid(),
                string.Empty,
                CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().BeNull(because: "the client id was not known");
        }

        /// <summary>
        /// Tests authenticating using only a refresh token.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        /// <remarks>
        /// This test is added to ensure that all other helper methods
        /// that do rely on <see cref="CreateRefreshTokenForUser"/> are not
        /// emitting false-positive.
        /// </remarks>
        [Fact]
        public async Task RefreshOnly()
        {
            var user = await this.identityInstance.CreateUserAsync(UserName).ConfigureAwait(true);

            var (_, tokenStr) = await this.CreateRefreshTokenForUser(user.Id)
                .ConfigureAwait(true);

            var authenticationResult = await this.authenticationFlow.TryAuthenticateUsingRefreshTokenAsync(
                tokenStr,
                false,
                this.identityInstance.ClientId,
                string.Empty,
                CancellationToken.None)
                .ConfigureAwait(true);

            authenticationResult.Should().NotBeNull(because: "the token was valid");
        }

        private async Task<(UserRefreshToken entity, string encoded)> CreateRefreshTokenForUser(
            Guid userId,
            Guid? appId = default,
            DateTimeOffset? expireAt = default,
            bool isInvalidated = false)
        {
            if (appId == default)
            {
                appId = this.identityInstance.ClientId;
            }

            var rng = this.randomGenerator.GetRandomBytes(64);

            var entity = new UserRefreshToken
            {
                ClientApplicationId = appId!.Value,
                GeneratedAtUtc = this.momentMock.Object.UtcNow,
                ExpiresAtUtc = expireAt ?? this.momentMock.Object.UtcNow.AddHours(1),
                SubjectUserId = userId,
                TokenValue = rng,
                IsInvalidated = isInvalidated,
                IpAddress = string.Empty,
                UserAgent = string.Empty,
            };

            this.dbContext.UserRefreshTokens.Add(entity);
            await this.dbContext.SaveChangesAsync().ConfigureAwait(true);

            return (entity, Convert.ToBase64String(rng));
        }

        private void ValidateBearer(string bearer, Guid userGuid, params string[] roles)
        {
            var handler = new JwtSecurityTokenHandler();
            var claimsPrincipal = handler.ValidateToken(
                bearer,
                new TokenValidationParameters()
                {
                    ValidIssuer = this.optionsMonitorMock.Object.CurrentValue.AuthNR.JWTAuthority,
                    ValidAudience = this.optionsMonitorMock.Object.CurrentValue.AuthNR.JWTAudience,
                    IssuerSigningKey = SigningKey,
                },
                out var _);
            this.identityInstance.UserManager.GetUserId(claimsPrincipal)
                .Should()
                .Be(
                    userGuid.ToString(),
                    because: "the token should contain the user id in the name identifier claim");

            foreach (var role in roles)
            {
                claimsPrincipal.IsInRole(role)
                    .Should()
                    .BeTrue(because: "The role should be added to the ClaimsPrincipal.");
            }
        }

        private async Task SetUserLockedOut(AppUser appUser)
        {
            // Identity may or may not do a round-trip on a user.
            // Therefore the change is applied to both the database and the user object.
            // In a real-world scenario, identity will always retrieve the latest info since
            // all objects are disposed after a request.
            appUser.LockoutEnd = DateTimeOffset.MaxValue;

            var userEntity = await this.dbContext.Users.FindAsync(appUser.Id).ConfigureAwait(true);
            userEntity.LockoutEnd = DateTimeOffset.MaxValue;
            await this.dbContext.SaveChangesAsync().ConfigureAwait(true);
        }
    }
}
