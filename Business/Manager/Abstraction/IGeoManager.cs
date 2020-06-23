// <copyright file="IGeoManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Abstraction
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Common.Models.Geo;

    /// <summary>
    /// Represents a manager that can perform geo-related operations.
    /// </summary>
    public interface IGeoManager
    {
        /// <summary>
        /// Tries to retrieve a map image based on a Dutch zip code.
        /// When no image is found, <c>null</c> is returned.
        /// </summary>
        /// <param name="dutchZipCode">The zip code.</param>
        /// <param name="houseNumber">The house number.</param>
        /// <param name="zoomLevel">The zoom level.</param>
        /// <param name="height">The height of the image or null.</param>
        /// <param name="width">The width of the image, or null.</param>
        /// <param name="cancellationToken">
        /// A token that can be used to request cancellation of the operation.
        /// </param>
        /// <returns>
        /// The image stream of the requested map image.
        /// The image will be a JPEG image.
        /// </returns>
        Task<Stream?> GetMapImageAsync(
            string dutchZipCode,
            int houseNumber,
            ZoomLevel zoomLevel,
            int? height,
            int? width,
            CancellationToken cancellationToken);
    }
}
