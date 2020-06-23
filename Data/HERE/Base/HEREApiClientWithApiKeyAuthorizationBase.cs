// <copyright file="HEREApiClientWithApiKeyAuthorizationBase.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.HERE.Base
{
    using System;
    using System.Collections.Immutable;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Fvect.Backend.Common.Exception;
    using Fvect.Backend.Common.Options;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Represents an abstract client for HERE APIs
    /// that use API Key Authorization.
    /// </summary>
    public abstract class HEREApiClientWithApiKeyAuthorizationBase
    {
        private static readonly string ApiKeyQueryParameter = "apiKey";

        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="HEREApiClientWithApiKeyAuthorizationBase"/>
        /// class.
        /// </summary>
        /// <param name="httpClient">The http client to use.</param>
        /// <param name="options">The options monitor to use.</param>
        /// <remarks>
        /// When other HTTP services are connected, it might be wise to refactor this
        /// into a generic client base class.
        /// </remarks>
        protected internal HEREApiClientWithApiKeyAuthorizationBase(
            HttpClient httpClient,
            IOptionsMonitor<BackendOptions> options)
        {
            this.client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.OptionsProvider = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Gets the options provider.
        /// </summary>
        protected IOptionsMonitor<BackendOptions> OptionsProvider { get; }

        /// <summary>
        /// Executes a request, converting <see cref="HttpRequestException"/> exceptions
        /// to <see cref="DataProviderException"/> exceptions.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        /// <param name="checkForSuccessStatusCode">
        /// A value indicating whether a check must be made if the request was succesfull.
        /// </param>
        /// <param name="notFoundIsSuccess">
        /// A value indicating if a 404 error should be seen as succes (no exception will be thrown in that case).
        /// </param>
        /// <returns>The return value.</returns>
        protected internal async Task<HttpResponseMessage> ExecConvertExceptionsAsync(
            Func<HttpClient, Task<HttpResponseMessage>> func,
            bool checkForSuccessStatusCode = false,
            bool notFoundIsSuccess = false)
        {
            try
            {
                func = func ?? throw new ArgumentNullException(nameof(func));
                var response = await func(this.client).ConfigureAwait(false);

                if (checkForSuccessStatusCode && !(notFoundIsSuccess && response.StatusCode == HttpStatusCode.NotFound))
                {
                    response.EnsureSuccessStatusCode();
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new DataProviderException(
                   "Error communicating with HERE API. See the inner exception for more details.",
                   ex);
            }
        }

        /// <summary>
        /// Gets the base <see cref="Uri"/> of the API. This includes the scheme and the host,
        /// but not the path.
        /// </summary>
        /// <returns>The base <see cref="Uri"/> of the API.</returns>
        protected internal abstract Uri GetBaseUri();

        /// <summary>
        /// Makes a URI to be used in a web request, that includes the API key.
        /// </summary>
        /// <param name="path">
        /// The path relative from the base <see cref="Uri"/> that is provided by
        /// <see cref="GetBaseUri"/>.
        /// </param>
        /// <param name="queryParameters">
        /// The query parameters. They will be URL encoded.
        /// </param>
        /// <returns>The <see cref="Uri"/> to use.</returns>
        protected internal Uri MakeUri(
            string path,
            ImmutableDictionary<string, string>? queryParameters)
        {
            queryParameters = (queryParameters ?? ImmutableDictionary<string, string>.Empty).Add(
                ApiKeyQueryParameter,
                this.GetApiKey());

            var queryBuilder = new StringBuilder();

            foreach (var (key, value) in queryParameters)
            {
                if (queryBuilder.Length > 0)
                {
                    queryBuilder.Append('&');
                }

                queryBuilder.Append(WebUtility.UrlEncode(key));
                queryBuilder.Append('=');
                queryBuilder.Append(WebUtility.UrlEncode(value));
            }

            return new UriBuilder(this.GetBaseUri())
            {
                Path = path,
                Query = queryBuilder.ToString(),
            }.Uri;
        }

        private string GetApiKey() => this.OptionsProvider.CurrentValue.Geo.HereMapsAPIKey;
    }
}
