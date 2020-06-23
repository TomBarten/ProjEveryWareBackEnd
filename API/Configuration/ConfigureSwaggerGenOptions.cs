// <copyright file="ConfigureSwaggerGenOptions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Provides functionality to configure Swagger Generator after the service container
    /// has been built. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// Inspired by https://github.com/microsoft/aspnet-api-versioning/blob/master/samples/aspnetcore/SwaggerSample/ConfigureSwaggerOptions.cs.
    /// </remarks>
    public sealed class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider apiProvider;
        private readonly string appName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerGenOptions"/> class.
        /// </summary>
        /// <param name="apiProvider">The API Version Description Provider.</param>
        public ConfigureSwaggerGenOptions(
            IApiVersionDescriptionProvider apiProvider)
        {
            this.apiProvider = apiProvider ?? throw new ArgumentNullException(nameof(apiProvider));
            this.appName = Assembly.GetExecutingAssembly().GetName()?.Name ?? string.Empty;
        }

        /// <inheritdoc />
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1513:Closing brace should be followed by blank line", Justification = "Looks awful.")]
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1500:Braces for multi-line statements should not share line", Justification = "Looks awful.")]
        public void Configure(SwaggerGenOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));

            foreach (var apiDescription in this.apiProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(apiDescription.GroupName, this.CreateInfoForApiVersion(apiDescription));

                if (!string.IsNullOrWhiteSpace(this.appName))
                {
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{this.appName}.xml"));
                }

                options.OperationFilter<SwaggerDefaultValues>();
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the bearer scheme. Put 'Bearer' in front of the token.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme,
                    },
                }] = new List<string>(),
            });
        }

        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription apiVersion)
        {
            var info = new OpenApiInfo()
            {
                Title = this.appName,
                Version = apiVersion.ApiVersion.ToString(),
                Description = $"OpenAPI documentation for \'{this.appName}\'.",
                Contact = new OpenApiContact() { Name = "FVect", Email = @"e24bbfbe.avans.onmicrosoft.com@emea.teams.ms" },
            };

            if (apiVersion.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
