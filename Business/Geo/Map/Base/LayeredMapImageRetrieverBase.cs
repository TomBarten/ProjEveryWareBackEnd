// <copyright file="LayeredMapImageRetrieverBase.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Geo.Map.Base
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Geo.Map.Abstraction;
    using Fvect.Backend.Common.Models.Geo;

    /// <summary>
    /// Represents an abstract implementation for <see cref="IMapImageRetriever"/>
    /// that will will use an inner retriever when it cannot retrieve the map image retriever
    /// itself.
    /// </summary>
    public abstract class LayeredMapImageRetrieverBase : IMapImageRetriever
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayeredMapImageRetrieverBase"/> class.
        /// </summary>
        /// <param name="inner">The inner retriever.</param>
        public LayeredMapImageRetrieverBase(
            IMapImageRetriever inner)
        {
            this.InnerRetriever = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        /// <summary>
        /// Gets the inner map image retriever.
        /// </summary>
        protected IMapImageRetriever InnerRetriever { get; }

        /// <summary>
        /// Gets or sets a handler that is invoked when a map image has been retrieved from the inner retriever.
        /// </summary>
        protected Func<(Stream image, string dutchZipCode, int houseNumber, ZoomLevel zoomLevel, CancellationToken ct), Task>?
            OnInnerRetrievedAsyncHandler
        { get; set; }

        /// <inheritdoc />
        public async Task<Stream?> GetMapImageAsync(
            string dutchZipCode,
            int houseNumber,
            ZoomLevel zoomLevel,
            CancellationToken cancellationToken)
        {
            var ownResult = await this.TryGetMapImage(
                dutchZipCode, houseNumber, zoomLevel, cancellationToken)
                .ConfigureAwait(false);

            if (!(ownResult is null))
            {
                return ownResult;
            }

            var innerResult = await this.InnerRetriever.GetMapImageAsync(
                    dutchZipCode, houseNumber, zoomLevel, cancellationToken)
                    .ConfigureAwait(false);

            if (!(innerResult is null) && !(this.OnInnerRetrievedAsyncHandler is null))
            {
                await this.OnInnerRetrievedAsyncHandler(
                    (innerResult, dutchZipCode, houseNumber, zoomLevel, cancellationToken))
                    .ConfigureAwait(false);
            }

            return innerResult;
        }

        /// <summary>
        /// Tries to retrieve a map image and returns <c>null</c>
        /// when it does not succeed.
        /// </summary>
        /// <param name="dutchZipCode">The zip code.</param>
        /// <param name="houseNumber">The house number.</param>
        /// <param name="zoomLevel">The zoom level.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The map image as JPEG stream.</returns>
        protected abstract Task<Stream?> TryGetMapImage(
            string dutchZipCode,
            int houseNumber,
            ZoomLevel zoomLevel,
            CancellationToken cancellationToken);
    }
}
