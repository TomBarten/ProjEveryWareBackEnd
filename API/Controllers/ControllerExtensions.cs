// <copyright file="ControllerExtensions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.Controllers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Contains extension methods for the <see cref="Controller"/> and <see cref="ControllerBase"/>
    /// types.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Creates a <see cref="StatusCodeResult"/> of which the value of the <see cref="StatusCodeResult.StatusCode"/>
        /// property is set to the status code corresponding to <see cref="HttpStatusCode.NotImplemented"/>.
        /// The reason phrase of the response is set to <c>Not Implemented.</c>.
        /// </summary>
        /// <param name="controllerBase">The target controller.</param>
        /// <returns>The created <see cref="StatusCodeResult"/>.</returns>
        [ExcludeFromCodeCoverage]
        public static StatusCodeResult NotImplemented(this ControllerBase controllerBase)
        {
            (controllerBase ?? throw new ArgumentNullException(nameof(controllerBase)))
                .HttpContext
                .Features
                .Get<IHttpResponseFeature>()
                .ReasonPhrase
                    = "Not Implemented";

            return controllerBase
                .StatusCode((int)HttpStatusCode.NotImplemented);
        }
    }
}
