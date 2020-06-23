// <copyright file="ServiceCollectionExtensions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Fvect.Backend.Common.Options;
    using Fvect.Backend.Data;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.HERE.Abstraction;
    using Fvect.Backend.Data.HERE.Implementation;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Contains functionality that can register the data layer
    /// to a service container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the FVect Data Layer to the given service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="databaseOptions">The application's database options.</param>
        /// <param name="identityBuilder">The identity builder for the application.</param>
        /// <returns>The value of <paramref name="serviceCollection"/> after the operation completed sucessfully, for chaining.</returns>
        public static IServiceCollection AddFVectDataLayer(
            this IServiceCollection serviceCollection,
            DatabaseOptions databaseOptions,
            IdentityBuilder identityBuilder)
        {
            serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            databaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));
            identityBuilder = identityBuilder ?? throw new ArgumentNullException(nameof(identityBuilder));

            // Database
            {
                serviceCollection.AddDbContext<FvectContext>((svp, options) => options
                    .UseSqlServer(databaseOptions.SQLServerConnectionString)
                    .UseLoggerFactory(svp.GetService<ILoggerFactory>()));

                // Identity
                {
                    identityBuilder.AddEntityFrameworkStores<FvectContext>();
                }
            }

            // HERE
            {
                serviceCollection
                    .AddHttpClient<IGeocodeClient, GeocodeClient>()
                    .AddPolicyHandler(PolicyRepository.DefaultExternalHttpDataSourcePolicy);

                serviceCollection
                    .AddHttpClient<IMapImageClient, MapImageClient>()
                    .AddPolicyHandler(PolicyRepository.DefaultExternalHttpDataSourcePolicy);
            }

            return serviceCollection;
        }
    }
}
