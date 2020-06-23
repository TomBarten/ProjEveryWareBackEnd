// <copyright file="ProgramTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.API;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Xunit;

    /// <summary>
    /// Contains tests for the <see cref="Program"/> class and the
    /// program dependency injection.
    /// </summary>
    public class ProgramTest
    {
        private static readonly string DevelopmentEnvironmentName = "Development";

        /// <summary>
        /// Tests that the application can be dry-ran.
        /// </summary>
        [Fact]
        public void CanDryRunApplication()
        {
            var expectedExitCode = 0;
            Assert.Equal(
                expectedExitCode,
                Program.Run(
                    Array.Empty<string>(),
                    dryRun: true));
        }

        /// <summary>
        /// Verifies that the application host created by the
        /// builder created by <see cref="Program.CreateHostBuilder(string[])"/>
        /// can provide all classes inside the 'Fvect.Backend.API' assembly that are
        /// not abstract, not generic and inherit from <see cref="ControllerBase"/>.
        /// </summary>
        /// <remarks>
        /// This implicitly validates all other services because every call
        /// to the application starts at a controller.
        /// </remarks>
        [Fact]
        public void CanConstructAllControllers()
        {
            var apiAssembly = typeof(Program).Assembly;

            var types = apiAssembly
                .GetTypes().ToList();

            var controllerTypes = apiAssembly
                .GetTypes()
                .Where(type =>
                    !type.IsAbstract
                    && type.IsSubclassOf(typeof(ControllerBase))
                    && !type.IsGenericType)
                .ToImmutableList();

            controllerTypes.Should()
                .HaveCountGreaterOrEqualTo(
                    1,
                    "because the application should contain at least one controller");

            var host = Program.CreateHostBuilder(Array.Empty<string>())
                .UseEnvironment(DevelopmentEnvironmentName)
                .Build();

            foreach (var controllerType in controllerTypes)
            {
                using var scope = host.Services.CreateScope();

                var controllerObj = scope.ServiceProvider.GetService(controllerType);

                controllerObj
                    .Should()
                    .BeOfType(
                        controllerType,
                        "because the controller should be instantiated by the service container");
            }
        }

        /// <summary>
        /// Tests that the application host can run without throwing an exception.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanRunWithoutThrowingAnException()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var host = Program.CreateHostBuilder(Array.Empty<string>()).Build();
            await host.StartAsync(cts.Token).ConfigureAwait(true);
            await Task.Delay(TimeSpan.FromSeconds(5), cts.Token).ConfigureAwait(true);
            await host.StopAsync(cts.Token).ConfigureAwait(true);
        }
    }
}
