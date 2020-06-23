// <copyright file="GeocodeClient.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.HERE.Implementation
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Common.Options;
    using Fvect.Backend.Data.HERE.Abstraction;
    using Fvect.Backend.Data.HERE.Base;
    using Fvect.Backend.Data.HERE.Models;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents a client for the Geocode search service.
    /// </summary>
    public class GeocodeClient : HEREApiClientWithApiKeyAuthorizationBase, IGeocodeClient
    {
        private static readonly string Path = "v1/geocode";

        private static readonly ImmutableDictionary<string, string> DefaultQueryParams =
            ImmutableDictionary<string, string>.Empty
                .Add("lang", "nl-NL")
                .Add("in", "countryCode:NLD");

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodeClient"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use.</param>
        /// <param name="options">The options.</param>
        public GeocodeClient(
            HttpClient httpClient,
            IOptionsMonitor<BackendOptions> options)
            : base(httpClient, options)
        {
        }

        /// <inheritdoc />
        public async Task<GeocodeInfo?> GetGeoCodeInfoForDutchPostalCodeAndHouseNumber(
            string dutchPostalCode,
            int? houseNumber,
            CancellationToken ct)
        {
            var qqValue = houseNumber != null
                ? $"postalCode={dutchPostalCode};houseNumber={houseNumber}"
                : $"postalCode={dutchPostalCode}";

            var uri = this.MakeUri(
                Path,
                DefaultQueryParams
                    .Add("qq", qqValue));

            var response = await this.ExecConvertExceptionsAsync(
                client => client.GetAsync(uri, ct),
                checkForSuccessStatusCode: true,
                notFoundIsSuccess: true)
                .ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            return JToken.Parse(
                await response.Content.ReadAsStringAsync().ConfigureAwait(false))["items"]
                .FirstOrDefault()?
                .ToObject<GeocodeInfo>();
        }

        /// <inheritdoc/>
        protected internal override Uri GetBaseUri() => this.OptionsProvider.CurrentValue.Geo.HEREGeocodeServiceBaseUri;
    }
}
