// <copyright file="AuthControllerTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.Controller.V1
{
    using System;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.API.AuthNR.Abstraction;
    using Fvect.Backend.API.V1.Controllers;
    using Fvect.Backend.API.V1.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    /// <summary>
    /// Represents a test for the <see cref="AuthController"/>.
    /// </summary>
    public class AuthControllerTest
    {
        /// <summary>
        /// Tests authentication attempt where the <see cref="IAuthenticationFlow"/> returns null.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AuthenticateFlowReturnsNullReturnsUnauthorized()
        {
            var (managerMock, flowMock, controller) = CreateControllerWithMocks();

            var username = "jtech";
            var psw = "pass";
            var includeToken = true;
            var clientAppId = Guid.NewGuid();
            var concurrencyStamp = string.Empty;
            var ct = CancellationToken.None;

            flowMock.Setup(m => m.TryAuthenticateUsingUserNameAndPasswordAsync(
                username,
                psw,
                includeToken,
                clientAppId,
                concurrencyStamp,
                ct))
                .Returns(Task.FromResult(null as AuthenticationResult))
                .Verifiable();

            var result = await controller.Authenticate(
                new AuthenticationRequest()
                {
                    UserName = username,
                    Password = psw,
                    IncludeRefreshToken = includeToken,
                    ClientApplicationId = clientAppId,
                    ConcurrencyStamp = concurrencyStamp,
                },
                ct).ConfigureAwait(true);

            result.Result.Should().BeOfType<UnauthorizedResult>(because: "the request was not authorized");

            flowMock.Verify();
            managerMock.Verify();

            flowMock.VerifyNoOtherCalls();
            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests authentication attempt where the <see cref="IAuthenticationFlow"/> returns null.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AuthenticateFlowReturnsResultReturnsResult()
        {
            var (managerMock, flowMock, controller) = CreateControllerWithMocks();

            var username = "jtech";
            var psw = "pass";
            var includeToken = true;
            var clientAppId = Guid.NewGuid();
            var concurrencyStamp = string.Empty;
            var ct = CancellationToken.None;
            var resultObj = new AuthenticationResult(
                username,
                psw,
                clientAppId,
                string.Empty,
                string.Empty,
                concurrencyStamp,
                default,
                default,
                string.Empty);

            flowMock.Setup(m => m.TryAuthenticateUsingUserNameAndPasswordAsync(
                    username,
                    psw,
                    includeToken,
                    clientAppId,
                    concurrencyStamp,
                    ct))
                .ReturnsAsync(resultObj)
                .Verifiable();

            var result = await controller.Authenticate(
                new AuthenticationRequest()
                {
                    UserName = username,
                    Password = psw,
                    IncludeRefreshToken = includeToken,
                    ClientApplicationId = clientAppId,
                    ConcurrencyStamp = concurrencyStamp,
                },
                ct).ConfigureAwait(true);

            result.Value.Should().BeSameAs(
                resultObj,
                because: "the manager returned a result");

            flowMock.Verify();
            managerMock.Verify();

            flowMock.VerifyNoOtherCalls();
            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests authentication attempt where the <see cref="IAuthenticationFlow"/> returns null.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task RefreshFlowReturnsNullReturnsUnauthorized()
        {
            var (managerMock, flowMock, controller) = CreateControllerWithMocks();

            var clientAppId = Guid.NewGuid();
            var refreshToken = string.Empty;
            var includeNewToken = false;
            var concurrencyStamp = string.Empty;
            var ct = CancellationToken.None;

            flowMock.Setup(m => m.TryAuthenticateUsingRefreshTokenAsync(
                    refreshToken,
                    includeNewToken,
                    clientAppId,
                    concurrencyStamp,
                    ct))
                .Returns(Task.FromResult(null as AuthenticationResult))
                .Verifiable();

            var result = await controller.Refresh(
                new RefreshRequest()
                {
                    ClientApplicationId = clientAppId,
                    RefreshToken = refreshToken,
                    IncludeNewRefreshToken = includeNewToken,
                    ConcurrencyStamp = concurrencyStamp,
                },
                ct).ConfigureAwait(true);

            result.Result.Should().BeOfType<UnauthorizedResult>(because: "the request was not authorized");

            flowMock.Verify();
            managerMock.Verify();

            flowMock.VerifyNoOtherCalls();
            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests authentication attempt where the <see cref="IAuthenticationFlow"/>
        /// returns an <see cref="AuthenticationResult"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task RefreshFlowReturnsResultReturnsResult()
        {
            var (managerMock, flowMock, controller) = CreateControllerWithMocks();

            var username = "jtech";
            var psw = "pass";
            var clientAppId = Guid.NewGuid();
            var refreshToken = string.Empty;
            var includeNewToken = false;
            var concurrencyStamp = string.Empty;
            var ct = CancellationToken.None;
            var resultObj = new AuthenticationResult(
                username,
                psw,
                clientAppId,
                string.Empty,
                string.Empty,
                concurrencyStamp,
                default,
                default,
                string.Empty);

            flowMock.Setup(m => m.TryAuthenticateUsingRefreshTokenAsync(
                    refreshToken,
                    includeNewToken,
                    clientAppId,
                    concurrencyStamp,
                    ct))
                .ReturnsAsync(resultObj)
                .Verifiable();

            var result = await controller.Refresh(
                new RefreshRequest()
                {
                    ClientApplicationId = clientAppId,
                    RefreshToken = refreshToken,
                    IncludeNewRefreshToken = includeNewToken,
                    ConcurrencyStamp = concurrencyStamp,
                },
                ct).ConfigureAwait(true);

            result.Value.Should().BeSameAs(
                resultObj,
                because: "the flow returned a result");

            flowMock.Verify();
            managerMock.Verify();

            flowMock.VerifyNoOtherCalls();
            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests user creation attempt where the <see cref="IAuthManager"/> returns null.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CreateUserManagerReturnsNullReturnsNoContent()
        {
            var (managerMock, flowMock, controller) = CreateControllerWithMocks();

            var username = "jtech";
            var psw = "pass";

            managerMock.Setup(m => m.CreateUserAsync(
                    username,
                    psw))
                .ReturnsAsync(null as string)
                .Verifiable();

            var result = await controller.CreateUser(
                new CreateUserRequest()
                {
                    UserName = username,
                    Password = psw,
                }).ConfigureAwait(true);

            result.Should().BeOfType<NoContentResult>(because: "the operation executed gracefully");

            flowMock.Verify();
            managerMock.Verify();

            flowMock.VerifyNoOtherCalls();
            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests user creation attempt where the <see cref="IAuthManager"/> returns a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task CreateUserManagerReturnsValueReturnsConflict()
        {
            var (managerMock, flowMock, controller) = CreateControllerWithMocks();

            var username = "jtech";
            var psw = "pass";
            var resultObj = "error";

            managerMock.Setup(m => m.CreateUserAsync(
                    username,
                    psw))
                .ReturnsAsync(resultObj)
                .Verifiable();

            var result = await controller.CreateUser(
                new CreateUserRequest()
                {
                    UserName = username,
                    Password = psw,
                }).ConfigureAwait(true);

            result.Should().BeOfType<ConflictObjectResult>(because: "an error was detected");

            flowMock.Verify();
            managerMock.Verify();

            flowMock.VerifyNoOtherCalls();
            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests token invalidation attempt where the <see cref="IAuthManager"/> returns true.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task InvalidateRefreshTokenManagerReturnsTrueReturnsNoContent()
        {
            var (managerMock, flowMock, controller) = CreateControllerWithMocks();

            var claimsPrincipal = new ClaimsPrincipal();
            var token = "wololo";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal,
                },
            };

            managerMock.Setup(m => m.InvalidateRefreshTokenAsync(
                    claimsPrincipal,
                    token))
                .ReturnsAsync(true)
                .Verifiable();

            var result = await controller.InvalidateRefreshToken(
                new RefreshTokenInvalidationRequest()
                {
                    Token = token,
                })
                .ConfigureAwait(true);

            result.Should().BeOfType<NoContentResult>(because: "the token was invalidated");

            flowMock.Verify();
            managerMock.Verify();

            flowMock.VerifyNoOtherCalls();
            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests token invalidation attempt where the <see cref="IAuthManager"/> returns false.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task InvalidateRefreshTokenManagerReturnsTrueReturnsNotFound()
        {
            var (managerMock, flowMock, controller) = CreateControllerWithMocks();

            var claimsPrincipal = new ClaimsPrincipal();
            var token = "wololo";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal,
                },
            };

            managerMock.Setup(m => m.InvalidateRefreshTokenAsync(
                    claimsPrincipal,
                    token))
                .ReturnsAsync(false)
                .Verifiable();

            var result = await controller.InvalidateRefreshToken(
                    new RefreshTokenInvalidationRequest()
                    {
                        Token = token,
                    })
                .ConfigureAwait(true);

            result.Should().BeOfType<NotFoundResult>(because: "the token was not found");

            flowMock.Verify();
            managerMock.Verify();

            flowMock.VerifyNoOtherCalls();
            managerMock.VerifyNoOtherCalls();
        }

        private static (
            Mock<IAuthManager> mock,
            Mock<IAuthenticationFlow> flowMock,
            AuthController controller) CreateControllerWithMocks()
        {
            var managerMock = new Mock<IAuthManager>();
            var flowMock = new Mock<IAuthenticationFlow>();
            var controller = new AuthController(flowMock.Object, managerMock.Object);

            return (managerMock, flowMock, controller);
        }
    }
}
