// <copyright file="PolicyRepository.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data
{
    using System;
    using System.Net.Http;
    using Polly;
    using Polly.Extensions.Http;

    /// <summary>
    /// Contains <see cref="Polly"/> policies.
    /// </summary>
    public static class PolicyRepository
    {
        /// <summary>
        /// Gets the default policy for external http data sources.
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> DefaultExternalHttpDataSourcePolicy =>
            HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, x => TimeSpan.FromMilliseconds(500 * Math.Pow(2, x)));
    }
}
