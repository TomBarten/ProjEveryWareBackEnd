// <copyright file="HttpTestHelper.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using Moq;

    /// <summary>
    /// Contains helper functionality for testing functionality
    /// that uses external HTTP calls.
    /// </summary>
    public static class HttpTestHelper
    {
        /// <summary>
        /// Creates a <see cref="Mock{T}"/> of type <see cref="HttpMessageHandler"/>
        /// with an <see cref="HttpClient"/> whose <see cref="HttpMessageHandler"/> is set
        /// to the <see cref="Mock{T}.Object"/> property of the <see cref="HttpMessageHandler"/>
        /// <see cref="Mock{T}"/>.
        /// </summary>
        /// <param name="mockInitializer">
        /// An initializer function that will be run when not <c>null</c> with the created mock
        /// passed in to the only parameter.
        /// </param>
        /// <returns>The created mock and client.</returns>
        public static (Mock<HttpMessageHandler> mock, HttpClient client) CreateMockWithClient(
            Action<Mock<HttpMessageHandler>>? mockInitializer = null)
        {
            var mock = new Mock<HttpMessageHandler>();
            mockInitializer?.Invoke(mock);
            return (mock, new HttpClient(mock.Object));
        }

        /// <summary>
        /// Checks a <see cref="Uri"/> against a specification
        /// of scheme, host, query parameters and port.
        /// </summary>
        /// <param name="target">The target <see cref="Uri"/>.</param>
        /// <param name="scheme">The expected scheme.</param>
        /// <param name="host">The expected host.</param>
        /// <param name="localPath">The expected local path.</param>
        /// <param name="queryParams">The expected HTTP query parameters.</param>
        /// <param name="port">The port.</param>
        /// <returns>A value indicating whether the query parameters adhere to the specification.</returns>
        public static bool CheckUri(
            Uri target,
            string scheme,
            string host,
            string localPath,
            ImmutableDictionary<string, string>? queryParams = default,
            uint? port = default)
        {
            if (target is null)
            {
                return false;
            }

            if (target.Scheme != (scheme ?? throw new ArgumentNullException(nameof(scheme))))
            {
                return false;
            }

            if (target.Host != (host ?? throw new ArgumentNullException(nameof(host))))
            {
                return false;
            }

            if (target.LocalPath != (localPath ?? throw new ArgumentNullException(nameof(localPath))))
            {
                return false;
            }

            if (queryParams != null)
            {
                var queryPairs = target.Query
                    .Remove(0, 1) // Remove the '?' in the beginning
                    .Split('&') // Split on parameters
                    .Select(x => x.Split('=')) // Split on key=value
                    .ToImmutableDictionary(x => x[0], x => x[1]);

                foreach (var (queryName, queryValue) in queryParams)
                {
                    if (!queryPairs.TryGetValue(WebUtility.UrlEncode(queryName), out var value))
                    {
                        return false;
                    }

                    if (value != WebUtility.UrlEncode(queryValue))
                    {
                        return false;
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(target.Query))
            {
                return false;
            }

            if (port != null)
            {
                return target.Port == port;
            }

            return target.IsDefaultPort;
        }
    }
}
