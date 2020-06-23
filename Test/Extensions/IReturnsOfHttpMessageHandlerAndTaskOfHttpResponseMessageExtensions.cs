// <copyright file="IReturnsOfHttpMessageHandlerAndTaskOfHttpResponseMessageExtensions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Moq
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Moq.Language;
    using Moq.Language.Flow;

    /// <summary>
    /// Contains extensions methods for instances of the <see cref="IReturns{TMock, TResult}"/>
    /// class, where the type of the mock is equal to <see cref="HttpMessageHandler"/> and
    /// the type of the result is equal to <see cref="HttpResponseMessage"/>.
    /// </summary>
    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "No unmanaged resources are created within scope.")]
    public static class IReturnsOfHttpMessageHandlerAndTaskOfHttpResponseMessageExtensions
    {
        /// <summary>
        /// Sets up the mock to return a string.
        /// </summary>
        /// <param name="returns">The returns.</param>
        /// <param name="content">The content.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns>The returns result.</returns>
        public static IReturnsResult<HttpMessageHandler> ReturnsStringBodyAsync(
            this IReturns<HttpMessageHandler, Task<HttpResponseMessage>> returns,
            string content,
            HttpStatusCode statusCode = HttpStatusCode.OK) => returns
                .ReturnsAsync(
                new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content),
                });

        /// <summary>
        /// Sets up the mock to return a stream.
        /// </summary>
        /// <param name="returns">The returns.</param>
        /// <param name="content">The content.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns>The returns result.</returns>
        public static IReturnsResult<HttpMessageHandler> ReturnsStreamBodyAsync(
            this IReturns<HttpMessageHandler, Task<HttpResponseMessage>> returns,
            Stream content,
            HttpStatusCode statusCode = HttpStatusCode.OK) => returns
                .ReturnsAsync(
                new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StreamContent(content),
                });
    }
}
