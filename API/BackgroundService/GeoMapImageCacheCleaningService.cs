// <copyright file="GeoMapImageCacheCleaningService.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.BackgroundService
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Task.Abstraction;
    using Fvect.Backend.Common.Options;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Represents a service that periodically executes the
    /// <see cref="IGeoMapImageCacheCleaningTask"/>.
    /// </summary>
    public class GeoMapImageCacheCleaningService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IOptionsMonitor<BackendOptions> optionsProvider;
        private readonly ILogger<GeoMapImageCacheCleaningService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMapImageCacheCleaningService"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory">The service scope factory.</param>
        /// <param name="optionsProvider">The options provider.</param>
        /// <param name="logger">The logger to use.</param>
        public GeoMapImageCacheCleaningService(
            IServiceScopeFactory serviceScopeFactory,
            IOptionsMonitor<BackendOptions> optionsProvider,
            ILogger<GeoMapImageCacheCleaningService> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory
                ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            this.optionsProvider = optionsProvider
                ?? throw new ArgumentNullException(nameof(optionsProvider));
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Exposes the <see cref="ExecuteAsync(CancellationToken)"/> method
        /// for testing purposes.
        /// </summary>
        /// <param name="stoppingToken">
        /// See the documentation of <see cref="ExecuteAsync(CancellationToken)"/>.
        /// </param>
        /// <returns>
        /// Refer to the documentation of <see cref="ExecuteAsync(CancellationToken)"/>.
        /// </returns>
        public Task RunAsync(CancellationToken stoppingToken) =>
            this.ExecuteAsync(stoppingToken);

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield(); // Execute asynchronously from here.

            stoppingToken.ThrowIfCancellationRequested();

            try
            {
                while (true)
                {
                    await this.Execute(stoppingToken).ConfigureAwait(false);
                    await Task.Delay(this.optionsProvider.CurrentValue.Geo.MapImageCacheEvictionInterval, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == stoppingToken)
            {
            }
        }

        [SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "Rethrowing the exception crashes the whole application.")]
        private async Task Execute(CancellationToken stoppingToken)
        {
            this.logger.LogDebug(
               $"Going to execute the {nameof(IGeoMapImageCacheCleaningTask)}.");

            try
            {
                using var scope = this.serviceScopeFactory.CreateScope();

                await scope.ServiceProvider.GetService<IGeoMapImageCacheCleaningTask>()
                    .RunAsync(stoppingToken)
                    .ConfigureAwait(false);

                this.logger.LogInformation(
                   $"Executed the {nameof(IGeoMapImageCacheCleaningTask)}.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    $"An error occured executing the {nameof(IGeoMapImageCacheCleaningTask)}. See the exception for more details.");
            }
        }
    }
}
