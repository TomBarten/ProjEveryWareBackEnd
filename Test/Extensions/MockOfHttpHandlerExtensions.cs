// <copyright file="MockOfHttpHandlerExtensions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Moq
{
    using System;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq.Language.Flow;
    using Moq.Protected;

    /// <summary>
    /// Contains extensions methods for instances of the <see cref="Mock{T}"/>
    /// class, where the type of the mock is equal to <see cref="HttpMessageHandler"/>.
    /// </summary>
    public static class MockOfHttpHandlerExtensions
    {
        private const string NameOfMockedMethod = "SendAsync";

        /// <summary>
        /// Creates a setup for the <see cref="HttpMessageHandler.SendAsync"/> method.
        /// </summary>
        /// <param name="mock">The target mock.</param>
        /// <param name="uriDelegate">A delegate to check the <see cref="Uri"/>.</param>
        /// <returns>The created <see cref="ISetup{TMock, TResult}"/>.</returns>
        public static ISetup<HttpMessageHandler, Task<HttpResponseMessage>> SetupHttpGet(
            this Mock<HttpMessageHandler> mock,
            Func<Uri, bool> uriDelegate) =>
            (mock ?? throw new ArgumentNullException(nameof(mock)))
                .SetupSendAsync(
                    MakeHttpRequestMessageExpression(uriDelegate ?? throw new ArgumentNullException(nameof(uriDelegate)), HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>());

        /// <summary>
        /// Creates a setup for the <see cref="HttpMessageHandler.SendAsync"/> method.
        /// </summary>
        /// <param name="mock">The target mock.</param>
        /// <param name="requestMessageExpression">The expression for the <see cref="HttpRequestMessage"/>.</param>
        /// <param name="cancellationTokenExpression">The expression for the <see cref="CancellationToken"/>.</param>
        /// <returns>The created <see cref="ISetup{TMock, TResult}"/>.</returns>
        public static ISetup<HttpMessageHandler, Task<HttpResponseMessage>> SetupSendAsync(
            this Mock<HttpMessageHandler> mock,
            Expression requestMessageExpression,
            Expression cancellationTokenExpression) =>
            (mock ?? throw new ArgumentNullException(nameof(mock)))
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    NameOfMockedMethod,
                    requestMessageExpression ?? throw new ArgumentNullException(nameof(requestMessageExpression)),
                    cancellationTokenExpression ?? throw new ArgumentNullException(nameof(cancellationTokenExpression)));

        private static Expression MakeHttpRequestMessageExpression(
            Func<Uri, bool> uriDelegate,
            HttpMethod httpMethodToExpect) => ItExpr.Is<HttpRequestMessage>(reqMsg =>
                reqMsg != null &&
                uriDelegate(reqMsg.RequestUri) &&
                httpMethodToExpect.Equals(reqMsg.Method));
    }
}
