// <copyright file="IMapImageRetriever.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Geo.Map.Abstraction
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Common.Models.Geo;

    /// <summary>
    /// Represents an object that can retrieve map images.
    /// </summary>
    public interface IMapImageRetriever
    {
        /// <summary>
        /// Tries to retrieve a map image. When the image is not
        /// available, <c>null</c> is returned.
        /// </summary>
        /// <param name="dutchZipCode">The zip code.</param>
        /// <param name="houseNumber">The house number.</param>
        /// <param name="zoomLevel">The zoom level.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The map image, or <c>null</c>.</returns>
        Task<Stream?> GetMapImageAsync(
            string dutchZipCode,
            int houseNumber,
            ZoomLevel zoomLevel,
            CancellationToken cancellationToken);
    }
}
