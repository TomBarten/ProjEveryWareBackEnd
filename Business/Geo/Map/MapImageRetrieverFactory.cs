// <copyright file="MapImageRetrieverFactory.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Geo.Map
{
    using System;
    using Fvect.Backend.Business.Geo.Map.Abstraction;
    using Fvect.Backend.Business.Geo.Map.Implementation;
    using Fvect.Backend.Common.Options;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.HERE.Abstraction;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Provides factory functionality for <see cref="IMapImageRetriever"/> instances.
    /// </summary>
    public static class MapImageRetrieverFactory
    {
        /// <summary>
        /// Creates the default <see cref="IMapImageRetriever"/>, which is a
        /// database cache that falls back to HERE services.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The default <see cref="IMapImageRetriever"/>.</returns>
        public static IMapImageRetriever CreateDefault(IServiceProvider serviceProvider)
        {
            var hereMapsRetriever = new HEREMapImageRetriever(
                serviceProvider.GetService<IGeocodeClient>(),
                serviceProvider.GetService<IMapImageClient>());

            var cacheRetriever = new CachedMapImageRetriever(
                serviceProvider.GetService<FvectContext>(),
                hereMapsRetriever,
                serviceProvider.GetService<IOptionsMonitor<BackendOptions>>(),
                serviceProvider.GetService<ILogger<CachedMapImageRetriever>>());

            return cacheRetriever;
        }
    }
}
