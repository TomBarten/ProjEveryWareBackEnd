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
    using Fvect.Backend.Common.Abstraction;
    using Fvect.Backend.Common.Implementation;
    using Fvect.Backend.Common.Options;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Contains functionality that can register the common layer
    /// to a service container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the FVect Common Layer to the given service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="appConfiguration">The application's <see cref="IConfiguration"/>.</param>
        /// <returns>The value of <paramref name="serviceCollection"/> after the operation completed sucessfully, for chaining.</returns>
        public static IServiceCollection AddFVectCommonLayer(this IServiceCollection serviceCollection, IConfiguration appConfiguration)
        {
            serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
            appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));

            serviceCollection.AddOptions();
            serviceCollection.Configure<BackendOptions>(appConfiguration);
            serviceCollection.AddTransient<IValidateOptions<BackendOptions>, BackendOptionsValidator>();

            serviceCollection.AddSingleton<IMoment, Moment>();
            serviceCollection.AddSingleton<ISecureRandomGenerator, SecureRandomGenerator>();

            return serviceCollection;
        }
    }
}
