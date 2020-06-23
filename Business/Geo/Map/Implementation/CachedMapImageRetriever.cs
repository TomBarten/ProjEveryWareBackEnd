// <copyright file="CachedMapImageRetriever.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Geo.Map.Implementation
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Geo.Map.Abstraction;
    using Fvect.Backend.Business.Geo.Map.Base;
    using Fvect.Backend.Common.Models.Geo;
    using Fvect.Backend.Common.Options;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Represents a layered map image retriever that uses a database as cache.
    /// </summary>
    public class CachedMapImageRetriever : LayeredMapImageRetrieverBase
    {
        private readonly FvectContext dbContext;
        private readonly IOptionsMonitor<BackendOptions> optionsProvider;
        private readonly ILogger<CachedMapImageRetriever> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedMapImageRetriever"/> class.
        /// </summary>
        /// <param name="dbContext">The database context to use.</param>
        /// <param name="inner">The inner map image retriever.</param>
        /// <param name="optionsProvider">The options provider.</param>
        /// <param name="logger">The logger.</param>
        public CachedMapImageRetriever(
            FvectContext dbContext,
            IMapImageRetriever inner,
            IOptionsMonitor<BackendOptions> optionsProvider,
            ILogger<CachedMapImageRetriever> logger)
            : base(inner)
        {
            this.OnInnerRetrievedAsyncHandler = this.HandleRetrievedFromInner;
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(logger));
            this.optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        protected override async Task<Stream?> TryGetMapImage(string dutchZipCode, int houseNumber, ZoomLevel zoomLevel, CancellationToken ct)
        {
            var zipCodeInCache = PostalCodeNormalization.NormalizeDutchPostalCode(
                dutchZipCode, zoomLevel);

            var doCache = this.CheckIfShouldBeCached(zoomLevel);

            if (doCache)
            {
                // Cachable. Check the cache.
                this.logger.LogDebug(
                    "Going to check cache for map image with zipCode {zipCodeInCache} and zoom level {zoomLevel}.",
                    zipCodeInCache,
                    zoomLevel);

                // Cast to nullable reference type as the call to the
                // database might return null.
                var imageInCache = (MapImage?)(await this.dbContext.MapImages
                    .FindAsync(new object[] { zipCodeInCache!, zoomLevel }, ct)
                    .ConfigureAwait(false));

                if (!(imageInCache is null))
                {
                    this.logger.LogDebug(
                        "Checked cache and found map image with zipCode {zipCodeInCache} and zoom level {zoomLevel}.",
                        zipCodeInCache,
                        zoomLevel);

                    this.logger.LogDebug(
                        "Going to set last access time for map image with zipCode {zipCodeInCache} and zoom level {zoomLevel}.",
                        zipCodeInCache,
                        zoomLevel);

                    try
                    {
                        imageInCache.LastTimeAccessed = DateTimeOffset.UtcNow;
                        await this.dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
                        this.logger.LogDebug(
                            "Set last access time for map image with zipCode {zipCodeInCache} and zoom level {zoomLevel}.",
                            zipCodeInCache,
                            zoomLevel);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        // Concurrency error can be ignored as this simply means
                        // that another API call accessed the image at the same time
                        // and also updated the last access time or the image was removed
                        // from the cache.
                        this.logger.LogDebug(
                            "Ignored concurrency error when setting access time for map image with zipCode {zipCodeInCache} and zoom level {zoomLevel}.",
                            zipCodeInCache,
                            zoomLevel);
                    }

                    return new MemoryStream(imageInCache.ImageData);
                }

                this.logger.LogDebug(
                    "Checked cache and did not find map image with zipCode {zipCodeInCache} and zoom level {zoomLevel}.", zipCodeInCache, zoomLevel);
            }
            else
            {
                this.logger.LogDebug(
                    "Not going to check cache for map image with zipCode {zipCode}, house number {houseNumber} and zoom level {zoomLevel}.",
                    zipCodeInCache,
                    houseNumber,
                    zoomLevel);
            }

            // Not cached or not cachable. Let the inner handler take care of it.
            return null;
        }

        private bool CheckIfShouldBeCached(ZoomLevel zoomLevel) =>
            this.optionsProvider.CurrentValue.Geo.CacheTimesPerZoomLevel[zoomLevel] > TimeSpan.Zero;

        private async Task HandleRetrievedFromInner((Stream image, string dutchZipCode, int houseNumber, ZoomLevel zoomLevel, CancellationToken ct) input)
        {
            var (image, zipCode, houseNumber, zoomLevel, ct) = input;

            var zipCodeInCache = PostalCodeNormalization.NormalizeDutchPostalCode(
                zipCode, zoomLevel);
            var doCache = this.CheckIfShouldBeCached(zoomLevel);

            if (doCache)
            {
                this.logger.LogDebug(
                    "Going to cache map image with zipCode {zipCodeInCache} and zoom level {zoomLevel}.",
                    zipCodeInCache,
                    zoomLevel);

                try
                {
                    using var ms = new MemoryStream(new byte[image.Length]);
                    await image.CopyToAsync(ms, ct).ConfigureAwait(false);
                    image.Position = 0;
                    ms.Position = 0;

                    var cacheEntry = new MapImage
                    {
                        PostalCode = zipCodeInCache!,
                        ZoomLevel = zoomLevel,
                        LastTimeAccessed = DateTimeOffset.UtcNow,
                        ImageData = ms.ToArray(),
                    };

                    this.dbContext.MapImages.Add(cacheEntry);

                    await this.dbContext.SaveChangesAsync(ct).ConfigureAwait(false);

                    this.logger.LogDebug(
                        "Cached map image with zipCode {zipCodeInCache} and zoom level {zoomLevel}.",
                        zipCodeInCache,
                        zoomLevel);
                }
                catch (DbUpdateException ex)
                {
                    this.logger.LogError(
                        ex,
                        "Error caching map image with zipCode {zipCodeInCache} and zoom level {zoomLevel}.",
                        zipCodeInCache,
                        zoomLevel);

                    // Ignore error: not critical to end user.
                }
            }
            else
            {
                this.logger.LogDebug(
                    "Not going to cache map image with zipCode {zipCode}, house number {houseNumber} and zoom level {zoomLevel}.",
                    zipCode,
                    houseNumber,
                    zoomLevel);
            }
        }
    }
}
