// <copyright file="Startup.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Text.Json.Serialization;
    using FluentValidation.AspNetCore;
    using Fvect.Backend.API.AuthNR.Abstraction;
    using Fvect.Backend.API.AuthNR.Implementation;
    using Fvect.Backend.API.BackgroundService;
    using Fvect.Backend.API.Configuration;
    using Fvect.Backend.API.JsonConversion;
    using Fvect.Backend.API.Middleware;
    using Fvect.Backend.Business;
    using Fvect.Backend.Common.Options;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Serilog;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Swashbuckle.AspNetCore.SwaggerUI;

    /// <summary>
    /// Provides functionality to bootstrap the application.
    /// </summary>
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Scope = "member",
        Justification = "ASP.NET Core runtime requires the members to be instance.")]
    [SuppressMessage(
        "ReSharper",
        "UnusedMember.Global",
        Justification = "Members called by ASP.NET Core runtime.")]
    public class Startup
    {
        [SuppressMessage("Code Quality", "IDE0052:Remove unread private members", Justification = "May be used in the future.")]
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Configures the service collection of the application.
        /// </summary>
        /// <param name="services">The service collection of the application.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var parsedConfiguration = this.configuration.Get<BackendOptions>();

            // Builders that are required by layers to register themselves
            IdentityBuilder identityBuilder;

            // General
            {
                services.AddHttpContextAccessor();
            }

            // CORS
            {
                services.AddCors(corsOptions => corsOptions
                    .AddDefaultPolicy(corsPolicy => corsPolicy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()));
            }

            // HSTS
            {
                services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(7);
                });
            }

            // AuthN / AuthR
            {
                // JWT Crypto
                {
                    services.AddSingleton<IJwtCryptoProvider, JwtCryptoProvider>();
                }

                // Identity
                {
                    identityBuilder = services.AddIdentityCore<AppUser>(opt =>
                    {
                        opt.Password.RequireDigit = false;
                        opt.Password.RequiredLength = 8;
                        opt.Password.RequireLowercase = false;
                        opt.Password.RequireUppercase = false;
                        opt.Password.RequireNonAlphanumeric = false;
                    })
                        .AddRoles<AppRole>()
                        .AddDefaultTokenProviders();

                    services.AddScoped<IAuthManager, AuthManager>();
                    services.AddScoped<IAuthenticationFlow, AuthenticationFlow>();
                }

                // AuthN
                {
                    services.AddAuthentication(
                        opt =>
                        {
                            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        })
                        .AddJwtBearer(
                            JwtBearerDefaults.AuthenticationScheme,
                            opt =>
                            {
                                opt.RequireHttpsMetadata = false;
                                opt.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidIssuer = parsedConfiguration.AuthNR.JWTAuthority,
                                    ValidAudience = parsedConfiguration.AuthNR.JWTAudience,
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(parsedConfiguration.AuthNR.JWTSigningKey)),
                                };
                            });
                }

                // AuthR
                {
                    services.AddAuthorization();
                }
            }

            // Client response cachiang
            {
                services.AddResponseCaching();
            }

            // API Versioning
            {
                services.AddApiVersioning(versioningOptions =>
                {
                    versioningOptions.ReportApiVersions = true;
                    versioningOptions.Conventions.Add(new VersionByNamespaceConvention());
                });

                services.AddVersionedApiExplorer(versionExplorerOptions =>
                {
                    // ReSharper disable once StringLiteralTypo
                    versionExplorerOptions.GroupNameFormat = "'v'VVV";
                    versionExplorerOptions.SubstituteApiVersionInUrl = true;
                });
            }

            // API Controllers
            {
                services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
                        options.JsonSerializerOptions.Converters.Add(new NullableTimeSpanConverter());
                    })
                    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                    .AddControllersAsServices(); // Used for testing purposes.
            }

            // OpenAPI
            {
                services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
                services.AddTransient<IConfigureOptions<SwaggerUIOptions>, ConfigureSwaggerUIOptions>();
                services.AddSwaggerGen();
            }

            // Background services
            {
                services.AddHostedService<GeoMapImageCacheCleaningService>();
            }

            // Application Layers
            {
                // When adding services to any of the application's layers outside of the API layer
                // that need to be provided through the service container, do not configure these services
                // here. Instead, configure the dependency injection inside the "Add{LayerName}Layer" method
                // extension method in the "ServiceCollectionExtensions" class in the root of the respective
                // application layer.
                services
                    .AddFVectCommonLayer(this.configuration) // Includes application options injection.
                    .AddFVectDataLayer(parsedConfiguration.Database, identityBuilder)
                    .AddFVectBusinessLayer();
            }
        }

        /// <summary>
        /// Builds the HTTP request pipeline of the application.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The webhost environment.</param>
        /// <param name="logger">The logger.</param>
        /// <remarks>
        /// This method configures an HTTP request pipeline, thus the order of method calls matter.
        /// </remarks>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app = app ?? throw new ArgumentNullException(nameof(app));
            env = env ?? throw new ArgumentNullException(nameof(env));
            logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Log the addresses that the application is listening on.
            logger.LogInformation(
                "Server listening on the following addresses: \'{@addresses}\'.",
                app.ServerFeatures.Get<IServerAddressesFeature>().Addresses);

            // Setup the request pipeline.
            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Do not enforce HTTPS in development.
                app.UseHsts();
                app.UseHttpsRedirection();

                // Do not catch Data Provider Exceptions in development
                // as the developer exception page will be more helpful
                // to the developer.
                app.UseMiddleware<DataProviderExceptionHandlerMiddleware>();
            }

            app.UseMiddleware<DataConflictExceptionHandlerMiddleware>();

            app.UseCors();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}
