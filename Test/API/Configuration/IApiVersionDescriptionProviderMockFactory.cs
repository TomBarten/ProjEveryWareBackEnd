// <copyright file="IApiVersionDescriptionProviderMockFactory.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.Configuration
{
    using System.Collections.Immutable;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Moq;

    /// <summary>
    /// Provides factory functionality for instances of
    /// <see cref="Mock{T}"/> where <c>T</c> is <see cref="IApiVersionDescriptionProvider"/>.
    /// </summary>
    public static class IApiVersionDescriptionProviderMockFactory
    {
        /// <summary>
        /// Creates a mock that contains a single API version.
        /// </summary>
        /// <returns>The mock containing a single API version.</returns>
        public static Mock<IApiVersionDescriptionProvider> CreateMock()
        {
            var mock = new Mock<IApiVersionDescriptionProvider>();

            mock.SetupGet(x => x.ApiVersionDescriptions)
                .Returns(
                    new[]
                    {
                        new ApiVersionDescription(
                            new ApiVersion(1, 1),
                            "v1",
                            false),
                    }.ToImmutableList());

            return mock;
        }
    }
}
