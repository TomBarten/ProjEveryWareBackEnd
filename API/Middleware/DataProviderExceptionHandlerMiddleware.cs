// <copyright file="DataProviderExceptionHandlerMiddleware.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;
    using Fvect.Backend.Common.Exception;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents middleware  that will return a status code result
    /// of type <see cref="HttpStatusCode.BadGateway"/> (502)
    /// to the client when an <see cref="DataProviderException"/> is thrown.
    /// </summary>
    public class DataProviderExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<DataProviderExceptionHandlerMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderExceptionHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next delegate.</param>
        /// <param name="logger">The logger.</param>
        public DataProviderExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<DataProviderExceptionHandlerMiddleware> logger)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous invocation.</returns>
        [DebuggerStepThrough]
        public async Task InvokeAsync(HttpContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            try
            {
                await this.next(context).ConfigureAwait(false);
            }
            catch (DataProviderException ex)
            {
                this.logger.LogError(
                    ex,
                    "Intercepted DataProviderException. See the exception for more details.");

                this.logger.LogDebug(
                    "Going to return a 502 (Bad Gateway) response...");

                context.Response.StatusCode = (int)HttpStatusCode.BadGateway;

                this.logger.LogInformation(
                    "Returned a 502 (Bad Gateway) response.");
            }
        }
    }
}
