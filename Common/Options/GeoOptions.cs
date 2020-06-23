// <copyright file="GeoOptions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Options
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Fvect.Backend.Common.Models.Geo;

    /// <summary>
    /// Represents the options of the geographics services
    /// of the application.
    /// </summary>
    /// <remarks>
    /// The non-nullable properties of this class are always set to null.
    /// This is not a problem because the <see cref="BackendOptionsValidator"/>
    /// checks that these values are not null.
    /// </remarks>
    public class GeoOptions
    {
        /// <summary>
        /// Gets or sets the API key that is to be used when communicating with
        /// HERE maps services.
        /// </summary>
        public string HereMapsAPIKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the base <see cref="Uri"/> of the HERE Geocode service.
        /// </summary>
        public Uri HEREGeocodeServiceBaseUri { get; set; } = null!;

        /// <summary>
        /// Gets or sets the base <see cref="Uri"/> of the HERE Map Image service.
        /// </summary>
        public Uri HEREMapImageServiceBaseUri { get; set; } = null!;

        /// <summary>
        /// Gets or sets the time interval at which the map image cache
        /// in the database is evicted. The default value is one hour.
        /// </summary>
        public TimeSpan MapImageCacheEvictionInterval { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// Gets or sets the maximum map image height in pixels that can be requested.
        /// The default value is 1440.
        /// </summary>
        public int MaxRequestHeightForMapImage { get; set; } = 1440;

        /// <summary>
        /// Gets or sets the maximum map image width in pixels that can be requested.
        /// The default value is 2560.
        /// </summary>
        public int MaxRequestWidthForMapImage { get; set; } = 2560;

        /// <summary>
        /// Gets or sets the default response height in pixels for a map image
        /// when the user did not specify a height. The default value is 720.
        /// </summary>
        public int DefaultResponseHeightForMapImage { get; set; } = 720;

        /// <summary>
        /// Gets or sets the default response width in pixels for a map image
        /// when the user did not specify a width. The default value is 1280.
        /// </summary>
        public int DefaultResponseWidthForMapImage { get; set; } = 1280;

        /// <summary>
        /// Gets or sets the height in pixels of map images that are requestes from the HERE
        /// Map Image Service. The default value is 1152 which is the maximum amount for
        /// map images requested from HERE in a <c>16:9</c> ratio.
        /// </summary>
        public int HERERequestHeightForMapImage { get; set; } = 1152;

        /// <summary>
        /// Gets or sets the width in pixels of map images that are requestes from the HERE
        /// Map Image Service. The default value is 2048 which is the maximum amount for
        /// map images requested from HERE in a <c>16:9</c> ratio.
        /// </summary>
        public int HERERequestWidthForMapImage { get; set; } = 2048;

        /// <summary>
        /// Gets or sets the cache time for a zoom level of a map image.
        /// When a value for a <see cref="ZoomLevel"/> is equal to <see cref="TimeSpan.Zero"/>,
        /// the map image will not be cached. Any value below <see cref="TimeSpan.Zero"/> is considered invalid and will
        /// cause a validation error.
        /// Default values are set to not cache <see cref="ZoomLevel.HOUSE"/> level, cache the <see cref="ZoomLevel.STREET"/>
        /// two days and cache all other <see cref="ZoomLevel"/> values two weeks.
        /// </summary>
        /// <remarks>
        /// When specifying own settings, ensure that a value is specified for
        /// every possible <see cref="ZoomLevel"/> value.
        /// </remarks>
        public IReadOnlyDictionary<ZoomLevel, TimeSpan> CacheTimesPerZoomLevel { get; set; } =
            ImmutableDictionary<ZoomLevel, TimeSpan>.Empty
                .Add(ZoomLevel.HOUSE, TimeSpan.Zero)
                .Add(ZoomLevel.STREET, TimeSpan.FromDays(2))
                .Add(ZoomLevel.NEIGHBOURHOOD, TimeSpan.FromDays(14))
                .Add(ZoomLevel.CITY, TimeSpan.FromDays(14))
                .Add(ZoomLevel.MUNICIPALITY, TimeSpan.FromDays(14))
                .Add(ZoomLevel.PROVINCE, TimeSpan.FromDays(14));
    }
}
