// <copyright file="UserProfileControllerTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.Controller.V1
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.API.V1.Controllers;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Common.Models;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Represents a test for the <see cref="UserProfileController"/>.
    /// </summary>
    public sealed class UserProfileControllerTest : IDisposable
    {
        private static readonly string UserName = "PietjePuk";
        private readonly ITestOutputHelper outputHelper;
        private readonly TestDatabase testDatabase;
        private readonly IdentityTestHelper.Instance identityInstance;
        private readonly HttpContext usedHttpContext;
        private readonly Mock<IUserProfileManager> managerMock;
        private readonly UserProfileController controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileControllerTest"/> class.
        /// </summary>
        /// <param name="helper">test output helper.</param>
        public UserProfileControllerTest(ITestOutputHelper helper)
        {
            this.outputHelper = helper;
            this.testDatabase = new TestDatabase(this.outputHelper);
            this.identityInstance = IdentityTestHelper.CreateInstance(this.testDatabase, this.outputHelper);
            this.usedHttpContext = new DefaultHttpContext();
            this.managerMock = new Mock<IUserProfileManager>();
            this.controller = new UserProfileController(this.managerMock.Object, this.identityInstance.UserManager)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = this.usedHttpContext,
                },
            };
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.identityInstance.Dispose();
            this.testDatabase.Dispose();
        }

        /// <summary>
        /// Tests getting an user profile by id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetUserProfileByIdTestUnauthorized()
        {
            var user = await this.CreateUserAndSignInAsync()
                .ConfigureAwait(true);
            var userProfile = new UserProfile { AppUserId = Guid.NewGuid() };

            this.managerMock.Setup(x => x.FindById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<UserProfile?>(Task.FromResult<UserProfile?>(userProfile)))
                .Verifiable();

            var result = await this.controller.GetEntity(userProfile.Id, default)
                .ConfigureAwait(true);

            this.managerMock.Verify();
            this.managerMock.VerifyNoOtherCalls();

            result.Result.Should().BeOfType<ForbidResult>(
                because: "the user does not own the profile");
        }

        /// <summary>
        /// Tests getting an user profile by id that doesn't exist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetUserProfileByIdNotFoundTest()
        {
            var result = await this.controller.GetEntity(Guid.NewGuid(), default)
                .ConfigureAwait(true);

            result.Result.Should().BeOfType<NotFoundResult>(
                because: "this profile does not exist.");
        }

        /// <summary>
        /// Tests updating a profile by id.
        /// </summary>
        /// <returns>A <see cref="Task"/> validates incorrect user.</returns>
        [Fact]
        public async Task UpdateUserProfileTestUnauthorized()
        {
            var user = await this.CreateUserAndSignInAsync()
                .ConfigureAwait(true);
            var userProfile = new UserProfile { AppUserId = Guid.NewGuid() };

            this.managerMock.Setup(x => x.FindById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile)
                .Verifiable();

            var result = await this.controller.PutEntity(userProfile.Id, userProfile, default)
                .ConfigureAwait(true);

            this.managerMock.Verify();
            this.managerMock.VerifyNoOtherCalls();

            result.Should().BeOfType<ForbidResult>(
                because: "the user does not own the profile");
        }

        /// <summary>
        /// Tests updating a profile by id.
        /// </summary>
        /// <returns>A <see cref="Task"/> validates incorrect profile.</returns>
        [Fact]
        public async Task UpdateUserProfileTestUnknownProfile()
        {
            var user = await this.CreateUserAndSignInAsync()
                .ConfigureAwait(true);
            var userProfile = new UserProfile { AppUserId = user.Id };

            this.managerMock.Setup(x => x.FindById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile)
                .Verifiable();

            var result = await this.controller.PutEntity(Guid.NewGuid(), userProfile, default)
                .ConfigureAwait(true);

            this.managerMock.Verify();

            result.Should().BeOfType<NotFoundResult>(
                because: "this profile does not exist.");
        }

        /// <summary>
        /// Tests adding an User Profile.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task AddUserProfilesTest()
        {
            var user = await this.CreateUserAndSignInAsync()
                .ConfigureAwait(true);
            var userProfile = new UserProfile();

            this.managerMock
                .Setup(userprofileManager => userprofileManager.Add(It.IsAny<UserProfile>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(userProfile.Id))
                .Verifiable();
            this.managerMock.Setup(x => x.FindById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile)
                .Verifiable();

            var result = await this.controller.PostEntity(userProfile, new ApiVersion(1, 0), default)
                .ConfigureAwait(true);

            var actionResult = result as CreatedAtActionResult;

            Assert.True(actionResult?.RouteValues.ContainsKey("id"));

            if (actionResult != null && actionResult.RouteValues.TryGetValue("id", out var idValue))
            {
                if (idValue is Guid guidValue)
                {
                   var profileResult = await this.controller.GetEntity(guidValue, default)
                        .ConfigureAwait(true);
                   profileResult.Value.AppUserId.Should().Be(user.Id);
                }
            }
        }

        /// <summary>
        /// Tests deleting an User Profile by id.
        /// </summary>
        /// <returns>A <see cref="Task"/> validates incorrect user.</returns>
        [Fact]
        public async Task DeleteUserProfileUnauthorized()
        {
            var user = await this.CreateUserAndSignInAsync()
                .ConfigureAwait(true);
            var userProfile = new UserProfile { AppUserId = Guid.NewGuid() };

            this.managerMock.Setup(x => x.FindById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile)
                .Verifiable();

            var result = await this.controller.DeleteEntity(userProfile.Id, default)
                .ConfigureAwait(true);

            this.managerMock.Verify();
            this.managerMock.VerifyNoOtherCalls();

            result.Result.Should().BeOfType<ForbidResult>(
                because: "the user does not own the profile");
        }

        /// <summary>
        /// Tests deleting an User Profile by id.
        /// </summary>
        /// <returns>A <see cref="Task"/> validates correct user.</returns>
        [Fact]
        public async Task DeleteUserProfileAuthorized()
        {
            var user = await this.CreateUserAndSignInAsync()
                .ConfigureAwait(true);
            var userProfile = new UserProfile { AppUserId = user.Id };

            this.managerMock.Setup(x => x.FindById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile)
                .Verifiable();

            this.managerMock.Setup(x => x.Remove(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Verifiable();

            var result = await this.controller.DeleteEntity(userProfile.Id, default)
                .ConfigureAwait(true);

            this.managerMock.Verify();
            this.managerMock.VerifyNoOtherCalls();

            result.Result.Should().BeOfType<NoContentResult>(
                because: "the user owned the profile");
        }

        private async Task<AppUser> CreateUserAndSignInAsync()
        {
            var user = await this.identityInstance.CreateUserAsync(UserName)
                .ConfigureAwait(false);
            var claimsPrincipal = await this.identityInstance.CreateClaimsPrincipalForUserAsync(user)
                .ConfigureAwait(false);

            this.usedHttpContext.User = claimsPrincipal;
            return user;
        }
    }
}
