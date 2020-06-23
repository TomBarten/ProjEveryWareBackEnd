// <copyright file="DataConflictExceptionHandlerMiddlewareTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.Middleware
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.API.Middleware;
    using Fvect.Backend.Common.Exception;
    using Microsoft.AspNetCore.Http;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Represents a test for the <see cref="DataConflictExceptionHandlerMiddleware"/>
    /// middleware.
    /// </summary>
    public class DataConflictExceptionHandlerMiddlewareTest
    {
        private readonly ITestOutputHelper outputHelper;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DataConflictExceptionHandlerMiddlewareTest"/> class.
        /// </summary>
        /// <param name="outputHelper">The test output helper.</param>
        public DataConflictExceptionHandlerMiddlewareTest(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
        }

        /// <summary>
        /// Tests that no effects are present when no exception is thrown.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution.</returns>
        [Fact]
        public async Task NoExceptionMiddlewareHasNoEffect()
        {
            var middleware = this.CreateMiddleware((context) => Task.CompletedTask);
            var context = new DefaultHttpContext();
            var originalStatusCode = context.Response.StatusCode;

            await middleware.InvokeAsync(context).ConfigureAwait(true);

            context.Response.StatusCode.Should()
                .Be(
                    originalStatusCode,
                    because: "it should not have been modified.");
        }

        /// <summary>
        /// Tests that standard exceptions are not caught.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution.</returns>
        [Fact]
        public async Task StandardExceptionMiddlewareHasNoEffect()
        {
            var middleware = this.CreateMiddleware((context) => Task.FromException(
                new SomeException()));

            var context = new DefaultHttpContext();

            Func<Task> act = () => middleware.InvokeAsync(context);

            await act.Should().ThrowExactlyAsync<SomeException>(
                because: "the thrown exception is not of type DataConflictException.")
                .ConfigureAwait(true);
        }

        /// <summary>
        /// Tests that <see cref="DataConflictException"/> exceptions are converted
        /// into a 409 Conflict response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution.</returns>
        [Fact]
        public async Task DataConflictExceptionMiddlewareCausesConflictResponse()
        {
            var middleware = this.CreateMiddleware((context) => Task.FromException(
                new DataConflictException()));

            var context = new DefaultHttpContext();

            await middleware.InvokeAsync(context).ConfigureAwait(true);

            context.Response.StatusCode.Should()
                .Be(
                    (int)HttpStatusCode.Conflict,
                    because: "DataConflictExceptions should be converted to a 409 Conflict response.");
        }

        private DataConflictExceptionHandlerMiddleware CreateMiddleware(
            RequestDelegate @delegate)
            => new DataConflictExceptionHandlerMiddleware(
                    @delegate,
                    LoggingTestHelper.CreateLogger<DataConflictExceptionHandlerMiddleware>(this.outputHelper));

        [SuppressMessage(
            "Design",
            "CA1032:Implement standard exception constructors",
            Justification = "Only used for testing.")]
        [SuppressMessage(
            "Design",
            "CA1064:Exceptions should be public",
            Justification = "Only used for testing.")]
        private class SomeException : Exception
        {
        }
    }
}
