// <copyright file="IMapImageClient.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.HERE.Abstraction
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Common.Exception;
    using Fvect.Backend.Common.Models.Geo;

    /// <summary>
    /// Represents a client for the HERE Map Image API.
    /// </summary>
    public interface IMapImageClient
    {
        /// <summary>
        /// Retrieves a map image.
        /// </summary>
        /// <param name="centerPosition">The center position of the map.</param>
        /// <param name="zoomLevel">The zoom level of the map.</param>
        /// <param name="height">The height of the map.</param>
        /// <param name="width">The width of the map.</param>
        /// <param name="ct">A token that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// The map image as a JPEG image stream.
        /// </returns>
        /// <exception cref="DataProviderException">
        /// When there was an error communicating with HERE Maps.
        /// </exception>
        Task<Stream> GetMapImage(
            Position centerPosition,
            int zoomLevel,
            int height,
            int width,
            CancellationToken ct);
    }
}
