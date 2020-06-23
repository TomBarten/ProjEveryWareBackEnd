// <copyright file="HEREMapImageRetriever.cs" company="Fvect">
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
    using Fvect.Backend.Common.Models.Geo;
    using Fvect.Backend.Data.HERE.Abstraction;

    /// <summary>
    /// Represents a retriever for map images that uses resources from HERE.
    /// </summary>
    public class HEREMapImageRetriever : IMapImageRetriever
    {
        private static readonly int HereMaxHeight = 1152;
        private static readonly int HereMaxWidth = 2048;

        private readonly IGeocodeClient geocodeClient;
        private readonly IMapImageClient mapImageClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HEREMapImageRetriever"/> class.
        /// </summary>
        /// <param name="geocodeClient">The geocode client to use.</param>
        /// <param name="mapImageClient">The map image client to use.</param>
        public HEREMapImageRetriever(
            IGeocodeClient geocodeClient,
            IMapImageClient mapImageClient)
        {
            this.geocodeClient = geocodeClient ?? throw new ArgumentNullException(nameof(geocodeClient));
            this.mapImageClient = mapImageClient ?? throw new ArgumentNullException(nameof(mapImageClient));
        }

        /// <summary>
        /// Maps a <see cref="ZoomLevel"/> value to a number that is
        /// passed to the HERE Map Image Service.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The number HERE understands.</returns>
        /// <remarks>
        /// This method is only made public for testing purposes.
        /// </remarks>
        public static int ZoomLevelToHereValue(ZoomLevel value) => value switch
        {
            ZoomLevel.HOUSE => 20,
            ZoomLevel.STREET => 18,
            ZoomLevel.NEIGHBOURHOOD => 16,
            ZoomLevel.CITY => 14,
            ZoomLevel.MUNICIPALITY => 13,
            ZoomLevel.PROVINCE => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(value)),
        };

        /// <inheritdoc/>
        public async Task<Stream?> GetMapImageAsync(
            string dutchZipCode, int houseNumber, ZoomLevel zoomLevel, CancellationToken cancellationToken)
        {
            var geoInfo = await this.geocodeClient.GetGeoCodeInfoForDutchPostalCodeAndHouseNumber(
                dutchZipCode,
                houseNumber,
                cancellationToken)
                .ConfigureAwait(false);

            if (geoInfo is null)
            {
                return null;
            }

            return await this.mapImageClient.GetMapImage(
                geoInfo.Position,
                ZoomLevelToHereValue(zoomLevel),
                HereMaxHeight,
                HereMaxWidth,
                cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
