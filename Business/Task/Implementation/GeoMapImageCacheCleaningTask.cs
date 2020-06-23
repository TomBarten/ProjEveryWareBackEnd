// <copyright file="GeoMapImageCacheCleaningTask.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Task.Implementation
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Task.Abstraction;
    using Fvect.Backend.Common.Options;
    using Fvect.Backend.Data.Database;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Represents a task that cleans up the cache of map images.
    /// </summary>
    public class GeoMapImageCacheCleaningTask : IGeoMapImageCacheCleaningTask
    {
        private readonly FvectContext dbContext;
        private readonly IOptionsMonitor<BackendOptions> optionsProvider;
        private readonly ILogger<GeoMapImageCacheCleaningTask> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMapImageCacheCleaningTask"/> class.
        /// </summary>
        /// <param name="dbContext">The database context to use.</param>
        /// <param name="optionsProvider">The options provider to use.</param>
        /// <param name="logger">The logger to use.</param>
        public GeoMapImageCacheCleaningTask(
            FvectContext dbContext,
            IOptionsMonitor<BackendOptions> optionsProvider,
            ILogger<GeoMapImageCacheCleaningTask> logger)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task RunAsync(CancellationToken ct)
        {
            this.logger.LogDebug("Going to evict map image cache.");

            // It might look counter-intuitive to do this in a foreach loop
            // but this allows a single delete query per zoom level to be sent to
            // the database server instead of loading all entities
            // into memory.
            foreach (var (zoomLevel, timeSpan) in this.optionsProvider.CurrentValue.Geo.CacheTimesPerZoomLevel)
            {
                var evictionDateTimeOffset = DateTimeOffset.UtcNow - timeSpan;

                this.dbContext.MapImages.RemoveRange(
                    this.dbContext.MapImages
                        .Where(img => img.ZoomLevel == zoomLevel && img.LastTimeAccessed < evictionDateTimeOffset));
            }

            await this.dbContext.SaveChangesAsync(ct).ConfigureAwait(false);
            this.logger.LogDebug("Evicted map image cache.");
        }
    }
}
