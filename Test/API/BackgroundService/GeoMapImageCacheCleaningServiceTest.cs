// <copyright file="GeoMapImageCacheCleaningServiceTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.BackgroundService
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.API.BackgroundService;
    using Fvect.Backend.Business.Task.Abstraction;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Contains tests for the <see cref="GeoMapImageCacheCleaningService"/> class.
    /// </summary>
    public class GeoMapImageCacheCleaningServiceTest
    {
        private readonly ITestOutputHelper outputHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMapImageCacheCleaningServiceTest"/> class.
        /// </summary>
        /// <param name="outputHelper">The output helper.</param>
        public GeoMapImageCacheCleaningServiceTest(
            ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper ??
                throw new ArgumentNullException(nameof(outputHelper));
        }

        /// <summary>
        /// Tests that the service triggers the underlying task at the interval
        /// specified in the options until the passed <see cref="CancellationToken"/>
        /// is triggered.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public Task RunsAtIntervalUntilCancellationTokenTriggered() => InvokeWithRetries(10, async () =>
        {
            var duration = TimeSpan.FromSeconds(2);
            var interval = TimeSpan.FromMilliseconds(500);
            var expectedInvocationCount = (int)(duration.TotalMilliseconds / interval.TotalMilliseconds);
            var tolerance = (uint)(expectedInvocationCount * .5);

            var (service, taskMock) = this.CreateServiceAndTaskMock(interval);

            var timesInvoked = 0;

            taskMock.Setup(t => t.RunAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() => timesInvoked++);

            using var cts = new CancellationTokenSource(duration);

            var serviceTask = service.RunAsync(cts.Token);

            Func<Task> waitFunc = async () => await serviceTask.ConfigureAwait(false);

            await waitFunc
                .Should()
                .NotThrowAsync(
                    because: "the infinite running service was cancelled")
                .ConfigureAwait(false);

            timesInvoked.Should()
                .BeCloseTo(
                    expectedInvocationCount,
                    tolerance,
                    because: "the task should have been invoked about this amount of times");
        });

        /// <summary>
        /// Tests that the <see cref="GeoMapImageCacheCleaningService"/> catches
        /// exceptions from the underlying task.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public Task CatchesExceptionsFromUnderlyingTask() => InvokeWithRetries(10, async () =>
        {
            var duration = TimeSpan.FromSeconds(2);
            var interval = TimeSpan.FromMilliseconds(500);
            var expectedInvocationCount = (int)(duration.TotalMilliseconds / interval.TotalMilliseconds);
            var tolerance = (uint)(expectedInvocationCount * .5);

            var (service, taskMock) = this.CreateServiceAndTaskMock(interval);

            var timesInvoked = 0;

            taskMock.Setup(t => t.RunAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromException(new Exception()))
                .Callback(() => timesInvoked++);

            using var cts = new CancellationTokenSource(duration);

            var serviceTask = service.RunAsync(cts.Token);

            Func<Task> waitFunc = async () => await serviceTask.ConfigureAwait(false);

            await waitFunc
                .Should()
                .NotThrowAsync(
                    because: "the infinite running service was cancelled")
                .ConfigureAwait(false);

            timesInvoked.Should()
                .BeCloseTo(
                    expectedInvocationCount,
                    tolerance,
                    because: "the task should have been invoked about this amount of times");
        });

        private static async Task InvokeWithRetries(
            int retryCount,
            Func<Task> code)
        {
            var attempts = 0;

            while (true)
            {
                try
                {
                    await code().ConfigureAwait(true);
                    return;
                }
                catch (Exception)
                {
                    if (attempts >= retryCount)
                    {
                        throw;
                    }
                }

                attempts++;
            }
        }

        private static (
            Mock<IServiceScopeFactory> serviceScopeFactoryMock,
            Mock<IServiceScope> serviceScopeMock,
            Mock<IServiceProvider> serviceProviderMock,
            Mock<IGeoMapImageCacheCleaningTask> taskMock)
            CreateMocks()
        {
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var serviceScopeMock = new Mock<IServiceScope>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            var taskMock = new Mock<IGeoMapImageCacheCleaningTask>();

            serviceScopeFactoryMock.Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);

            serviceScopeMock.SetupGet(x => x.ServiceProvider)
                .Returns(serviceProviderMock.Object);

            serviceProviderMock.Setup(x => x.GetService(typeof(IGeoMapImageCacheCleaningTask)))
                .Returns(taskMock.Object);

            return (
                serviceScopeFactoryMock,
                serviceScopeMock,
                serviceProviderMock,
                taskMock);
        }

        private (
            GeoMapImageCacheCleaningService service,
            Mock<IGeoMapImageCacheCleaningTask> taskMock)
            CreateServiceAndTaskMock(TimeSpan interval)
        {
            var optionsMock = OptionsTestHelper.CreateBackendOptionsMock(
                x => x.Geo.MapImageCacheEvictionInterval = interval);

            var logger = LoggingTestHelper.CreateLogger<GeoMapImageCacheCleaningService>(
                this.outputHelper);

            var (svcScopeFact, _, _, taskMock) = CreateMocks();

            return (
                new GeoMapImageCacheCleaningService(
                    svcScopeFact.Object,
                    optionsMock.Object,
                    logger),
                taskMock);
        }
    }
}
