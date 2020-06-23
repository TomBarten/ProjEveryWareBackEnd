// <copyright file="OptionsTestHelper.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fvect.Backend.Common.Options;
    using Microsoft.Extensions.Options;
    using Moq;

    /// <summary>
    /// Contains helper functions for options during tests.
    /// </summary>
    public static class OptionsTestHelper
    {
        /// <summary>
        /// Creates a <see cref="Mock{T}"/> of type <see cref="IOptionsMonitor{TOptions}"/>
        /// of type <see cref="BackendOptions"/> that will return the options created by
        /// <see cref="CreateValidOptions"/>.
        /// </summary>
        /// <param name="configurator">A mutator to execute on the options.</param>
        /// <returns>The created mock.</returns>
        public static Mock<IOptionsMonitor<BackendOptions>> CreateBackendOptionsMock(
            Action<BackendOptions>? configurator = null)
        {
            var options = CreateValidOptions();
            configurator?.Invoke(options);

            var mock = new Mock<IOptionsMonitor<BackendOptions>>();
            mock.SetupGet(x => x.CurrentValue).Returns(options);

            return mock;
        }

        /// <summary>
        /// Creates a valid instance of the <see cref="BackendOptions"/> class for test usage.
        /// </summary>
        /// <returns>The created <see cref="BackendOptions"/> instance.</returns>
        public static BackendOptions CreateValidOptions() =>
                new BackendOptions()
                {
                    Database = new DatabaseOptions
                    {
                        SQLServerConnectionString = @"Server=localhost;Database=DoesNotExist;Trusted_Connection=True;",
                    },
                    Geo = new GeoOptions
                    {
                        HereMapsAPIKey = "fake_API_key",
                        HEREGeocodeServiceBaseUri = new Uri("https://example.com/"),
                        HEREMapImageServiceBaseUri = new Uri("https://example.com/"),
                    },
                    AuthNR = new AuthNROptions
                    {
                        JWTAudience = "https://localhost",
                        JWTAuthority = "https://localhost",
                        JWTSigningKey = "This is the test signing key. Do not use me in production.",
                    },
                };
    }
}
