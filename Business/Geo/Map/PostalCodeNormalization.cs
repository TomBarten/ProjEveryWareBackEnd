// <copyright file="PostalCodeNormalization.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Geo.Map
{
    using System;
    using Fvect.Backend.Common.Models.Geo;

    /// <summary>
    /// Provides a function to normalize postal codes for a zoomlevel.
    /// </summary>
    public static class PostalCodeNormalization
    {
        /// <summary>
        /// Nomalizes a dutch postal code and removes the
        /// letters when this is no longer relevant for the zoom level.
        /// </summary>
        /// <param name="dutchPostalCode">The postal code to normalize.</param>
        /// <param name="zoomLevel">The zoomlevel.</param>
        /// <returns>The normalized postal code.</returns>
        public static string NormalizeDutchPostalCode(
            string dutchPostalCode,
            ZoomLevel zoomLevel)
        {
            dutchPostalCode = dutchPostalCode?.Replace(
                " ",
                string.Empty,
                StringComparison.InvariantCultureIgnoreCase)
                ?? throw new ArgumentNullException(nameof(dutchPostalCode));

            return zoomLevel switch
            {
                ZoomLevel.HOUSE => dutchPostalCode,
                ZoomLevel.STREET => dutchPostalCode,
                _ => dutchPostalCode.Substring(0, 4),
            };
        }
    }
}
