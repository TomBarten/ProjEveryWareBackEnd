// <copyright file="MapImageClient.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.HERE.Implementation
{
    using System;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Common.Models.Geo;
    using Fvect.Backend.Common.Options;
    using Fvect.Backend.Data.HERE.Abstraction;
    using Fvect.Backend.Data.HERE.Base;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Represents a client for the Geocode search service.
    /// </summary>
    public class MapImageClient : HEREApiClientWithApiKeyAuthorizationBase, IMapImageClient
    {
        private static readonly string Path = "mia/1.6/mapview";

        private static readonly ImmutableDictionary<string, string> DefaultQueryParams =
            ImmutableDictionary<string, string>.Empty
                .Add("ml", "dut") // Dutch map markers
                .Add("f", "1") // JPEG format
                .Add("ppi", "250") // 250 pixels per inch
                .Add("nodot", "true"); // No map dot

        /// <summary>
        /// Initializes a new instance of the <see cref="MapImageClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use.</param>
        /// <param name="options">The options.</param>
        public MapImageClient(
            HttpClient httpClient,
            IOptionsMonitor<BackendOptions> options)
            : base(httpClient, options)
        {
        }

        /// <inheritdoc />
        public async Task<Stream> GetMapImage(
            Position centerPosition,
            int zoomLevel,
            int height,
            int width,
            CancellationToken ct)
        {
            var uri = this.MakeUri(
                Path,
                DefaultQueryParams
                    .Add("c", centerPosition.ToString())
                    .Add("z", zoomLevel.ToString(CultureInfo.InvariantCulture))
                    .Add("h", height.ToString(CultureInfo.InvariantCulture))
                    .Add("w", width.ToString(CultureInfo.InvariantCulture)));

            var response = await this.ExecConvertExceptionsAsync(
                client => client.GetAsync(uri, ct),
                checkForSuccessStatusCode: true)
                .ConfigureAwait(false);

            return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected internal override Uri GetBaseUri() => this.OptionsProvider.CurrentValue.Geo.HEREMapImageServiceBaseUri;
    }
}
