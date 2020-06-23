// <copyright file="IdentityTestHelper.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    /// <summary>
    /// Contains helper functions for identity and AuthNR during tests.
    /// </summary>
    public static class IdentityTestHelper
    {
        /// <summary>
        /// Creates a <see cref="ClaimsPrincipal"/> that represents a user
        /// by their id.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>The created <see cref="ClaimsPrincipal"/>.</returns>
        public static ClaimsPrincipal CreateClaimsPrincipalForUserId(
            Guid id) =>
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>()
                    {
                        new Claim(
                            ClaimTypes.NameIdentifier,
                            id.ToString()),
                    },
                    nameof(IdentityTestHelper)));

        /// <summary>
        /// Creates an identity instance.
        /// </summary>
        /// <param name="testDb">The test database.</param>
        /// <param name="outputHelper">The test output helper.</param>
        /// <returns>The identity instance.</returns>
        public static Instance CreateInstance(
            TestDatabase testDb,
            ITestOutputHelper outputHelper)
        {
            var svc = new ServiceCollection();

            svc.AddLogging(builder => builder.AddXUnit(outputHelper));
            svc.AddScoped(svp => testDb.CreateContext());
            svc.AddDataProtection();

            svc
                .AddIdentityCore<AppUser>(opt =>
                {
                    opt.Password.RequireDigit = false;
                    opt.Password.RequiredLength = 1;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                })
                    .AddRoles<AppRole>()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<FvectContext>();

            return new Instance(svc.BuildServiceProvider());
        }

        /// <summary>
        /// Represents an instance of the identity framework.
        /// </summary>
        [SuppressMessage(
            "Design", "CA1034:Nested types should not be visible", Justification = "Clarity")]
        public sealed class Instance : IDisposable
        {
            /// <summary>
            /// The default password of users created by helper methods of this <see cref="Instance"/>.
            /// </summary>
            public const string DefaultPassword = "pwd";

            private readonly IServiceScope scope;

            /// <summary>
            /// Initializes a new instance of the <see cref="Instance"/> class.
            /// </summary>
            /// <param name="svp">The service provider.</param>
            public Instance(
                IServiceProvider svp)
            {
                this.scope = svp.CreateScope();
                this.UserManager = this.scope.ServiceProvider.GetService<UserManager<AppUser>>();
                this.SignInManager = this.scope.ServiceProvider.GetService<SignInManager<AppUser>>();
                this.RoleManager = this.scope.ServiceProvider.GetService<RoleManager<AppRole>>();

                using var tempScope = svp.CreateScope();
                var context = tempScope.ServiceProvider.GetService<FvectContext>();
                var clientEntity = new ClientApplication
                {
                    Name = "XUnit",
                    Id = this.ClientId,
                };
                if (!context.ClientApplications.Where(c => c.Id == this.ClientId).Any())
                {
                    context.ClientApplications.Add(clientEntity);
                    context.SaveChanges();
                }
            }

            /// <summary>
            /// Gets the identifier of a client registered at this instance.
            /// </summary>
            public Guid ClientId { get; } = Guid.NewGuid();

            /// <summary>
            /// Gets the user manager.
            /// </summary>
            public UserManager<AppUser> UserManager { get; }

            /// <summary>
            /// Gets the sign in manager.
            /// </summary>
            public SignInManager<AppUser> SignInManager { get; }

            /// <summary>
            /// Gets the role manager.
            /// </summary>
            public RoleManager<AppRole> RoleManager { get; }

            /// <summary>
            /// Creates a user with the specified username and password.
            /// </summary>
            /// <param name="userName">The username.</param>
            /// <param name="password">The password.</param>
            /// <returns>The created user.</returns>
            public async Task<AppUser> CreateUserAsync(string userName, string password = DefaultPassword)
            {
                var result = await this.UserManager.CreateAsync(
                    new AppUser
                    {
                        UserName = userName,
                    },
                    password)
                    .ConfigureAwait(false);

                if (result.Errors.Any())
                {
                    throw new Exception("An error occured creating the test user. Check the test output for details");
                }

                return await this.UserManager.FindByNameAsync(userName).ConfigureAwait(false);
            }

            /// <summary>
            /// Creates a <see cref="ClaimsPrincipal"/> for a user that is authenticated using
            /// '<c>TestAuthentication</c>'.
            /// </summary>
            /// <param name="user">The user to create the <see cref="ClaimsPrincipal"/> for.</param>
            /// <returns>The created <see cref="ClaimsPrincipal"/>.</returns>
            public async Task<ClaimsPrincipal> CreateClaimsPrincipalForUserAsync(AppUser user)
            {
                var identityUserClaims = await this.UserManager.GetClaimsAsync(user).ConfigureAwait(false);
                var identityRoles = await this.UserManager.GetRolesAsync(user).ConfigureAwait(false);
                var identityRoleClaims = identityRoles.Select(role => new Claim(ClaimTypes.Role, role));

                var claims = Enumerable.Empty<Claim>()
                    .Append(new Claim(ClaimTypes.Name, user.UserName))
                    .Append(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()))
                    .Concat(identityUserClaims)
                    .Concat(identityRoleClaims);

                return new ClaimsPrincipal(
                    new[]
                    {
                        new ClaimsIdentity(
                            claims,
                            "TestAuthentication"),
                    });
            }

            /// <inheritdoc />
            public void Dispose() => this.scope.Dispose();
        }
    }
}
