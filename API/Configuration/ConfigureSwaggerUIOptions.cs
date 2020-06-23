// <copyright file="ConfigureSwaggerUIOptions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.Configuration
{
    using System;
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Options;
    using Swashbuckle.AspNetCore.SwaggerUI;

    /// <summary>
    /// Provides functionality to configure Swagger UI after the service container
    /// has been built. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// Inspired by https://github.com/microsoft/aspnet-api-versioning/blob/master/samples/aspnetcore/SwaggerSample/ConfigureSwaggerOptions.cs.
    /// </remarks>
    public class ConfigureSwaggerUIOptions : IConfigureOptions<SwaggerUIOptions>
    {
        private readonly IApiVersionDescriptionProvider apiProvider;
        private readonly string appName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerUIOptions"/> class.
        /// </summary>
        /// <param name="apiProvider">The API provider.</param>
        public ConfigureSwaggerUIOptions(IApiVersionDescriptionProvider apiProvider)
        {
            this.apiProvider = apiProvider ?? throw new ArgumentNullException(nameof(apiProvider));
            this.appName = Assembly.GetExecutingAssembly().GetName()?.Name ?? string.Empty;
        }

        /// <inheritdoc />
        public void Configure(SwaggerUIOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));

            options.RoutePrefix = "docs";
            options.DocumentTitle = $"\'{this.appName}\' technical documentation";
            options.DocExpansion(DocExpansion.List);
            options.DefaultModelExpandDepth(0);

            foreach (var apiDescription in this.apiProvider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{apiDescription.GroupName}/swagger.json", apiDescription.GroupName);
            }
        }
    }
}
