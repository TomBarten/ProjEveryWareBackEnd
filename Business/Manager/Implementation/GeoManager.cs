// <copyright file="GeoManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Implementation
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Geo.Map.Abstraction;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Common.Models.Geo;
    using Fvect.Backend.Common.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Represents a manager for geo-related operations.
    /// </summary>
    public class GeoManager : IGeoManager
    {
        private readonly IMapImageRetriever imageRetriever;
        private readonly IImageResizingStrategy resizer;
        private readonly IOptionsMonitor<BackendOptions> optionsProvider;
        private readonly ILogger<GeoManager> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoManager"/> class.
        /// </summary>
        /// <param name="imageRetriever">The image retriever.</param>
        /// <param name="resizer">The resizer.</param>
        /// <param name="optionsProvider">The options provider.</param>
        /// <param name="logger">The logger.</param>
        public GeoManager(
            IMapImageRetriever imageRetriever,
            IImageResizingStrategy resizer,
            IOptionsMonitor<BackendOptions> optionsProvider,
            ILogger<GeoManager> logger)
        {
            this.imageRetriever = imageRetriever ?? throw new ArgumentNullException(nameof(imageRetriever));
            this.resizer = resizer ?? throw new ArgumentNullException(nameof(resizer));
            this.optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<Stream?> GetMapImageAsync(
            string dutchZipCode,
            int houseNumber,
            ZoomLevel zoomLevel,
            int? height,
            int? width,
            CancellationToken cancellationToken)
        {
            var actHeight = height ?? this.optionsProvider.CurrentValue.Geo.DefaultResponseHeightForMapImage;
            var actWidth = width ?? this.optionsProvider.CurrentValue.Geo.DefaultResponseWidthForMapImage;

            this.logger.LogDebug(
                "Going to retrieve image for zip code {zipCode}, house number {houseNumber} and zoom level {zoomLevel}.",
                dutchZipCode,
                houseNumber,
                zoomLevel);

            using var image = await this.imageRetriever.GetMapImageAsync(
                dutchZipCode,
                houseNumber,
                zoomLevel,
                cancellationToken)
                .ConfigureAwait(false);

            if (image is null)
            {
                this.logger.LogDebug(
                   "Could not retrieve image for zip code {zipCode}, house number {houseNumber} and zoom level {zoomLevel}.",
                   dutchZipCode,
                   houseNumber,
                   zoomLevel);

                return null;
            }

            this.logger.LogDebug(
                "Retrieved image for zip code {zipCode}, house number {houseNumber} and zoom level {zoomLevel}.",
                dutchZipCode,
                houseNumber,
                zoomLevel);

            this.logger.LogDebug(
                "Going to resize image for zip code {zipCode}, house number {houseNumber} and zoom level {zoomLevel} to height {height} and width {width}.",
                dutchZipCode,
                houseNumber,
                zoomLevel,
                actHeight,
                actWidth);

            var ms = new MemoryStream();

            this.resizer.ResizeJPEGImage(
                image,
                ms,
                actHeight,
                actWidth);

            this.logger.LogDebug(
                "Resized image for zip code {zipCode}, house number {houseNumber} and zoom level {zoomLevel} to height {height} and width {width}.",
                dutchZipCode,
                houseNumber,
                zoomLevel,
                actHeight,
                actWidth);

            return ms;
        }
    }
}
