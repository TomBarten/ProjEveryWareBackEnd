// <copyright file="GeoMapImageCacheCleaningTaskTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Business.Task
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.Business.Task.Implementation;
    using Fvect.Backend.Common.Models.Geo;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.EntityFrameworkCore;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Contains tests for the <see cref="GeoMapImageCacheCleaningTask"/> class.
    /// </summary>
    public class GeoMapImageCacheCleaningTaskTest
    {
        private readonly ITestOutputHelper outputHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMapImageCacheCleaningTaskTest"/> class.
        /// </summary>
        /// <param name="outputHelper">The test output helper.</param>
        public GeoMapImageCacheCleaningTaskTest(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
        }

        /// <summary>
        /// Tests that images are not evicted when their last access time is within the treshold.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous test execution.</returns>
        [Fact]
        public async Task DoesNotEvictWithinCachePeriod()
        {
            var evictionPeriod = TimeSpan.FromDays(14);
            var imageAccessTime = DateTimeOffset.UtcNow - (evictionPeriod - TimeSpan.FromDays(2));
            var postalCode = "1234";
            var zoomLevel = ZoomLevel.CITY;

            var optionsMock = OptionsTestHelper.CreateBackendOptionsMock(
                x => x.Geo.CacheTimesPerZoomLevel = ImmutableDictionary<ZoomLevel, TimeSpan>.Empty
                    .Add(zoomLevel, evictionPeriod));

            using var testDb = new TestDatabase(this.outputHelper);

            using (var ctx = await testDb.CreateContextAsync().ConfigureAwait(true))
            {
                ctx.MapImages.Add(
                    new MapImage
                    {
                        ImageData = new byte[] { 0x01 },
                        LastTimeAccessed = imageAccessTime,
                        PostalCode = postalCode,
                        ZoomLevel = zoomLevel,
                    });
                await ctx.SaveChangesAsync().ConfigureAwait(true);
            }

            using (var ctx = await testDb.CreateContextAsync().ConfigureAwait(true))
            {
                var task = new GeoMapImageCacheCleaningTask(
                    ctx,
                    optionsMock.Object,
                    LoggingTestHelper.CreateLogger<GeoMapImageCacheCleaningTask>(this.outputHelper));

                await task.RunAsync(default).ConfigureAwait(true);
            }

            using (var ctx = await testDb.CreateContextAsync().ConfigureAwait(true))
            {
                (await ctx.MapImages.CountAsync().ConfigureAwait(true))
                    .Should()
                    .Be(
                        1,
                        because: "the only image in the cache should not have been evicted.");
            }
        }

        /// <summary>
        /// Tests that images are evicted when their last access time is outside of the treshold.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous test execution.</returns>
        [Fact]
        public async Task DoesEvictOutOfCachePeriod()
        {
            var evictionPeriod = TimeSpan.FromDays(14);
            var imageAccessTime = DateTimeOffset.UtcNow - (evictionPeriod + TimeSpan.FromDays(2));
            var postalCode = "1234";
            var zoomLevel = ZoomLevel.CITY;

            var optionsMock = OptionsTestHelper.CreateBackendOptionsMock(
                x => x.Geo.CacheTimesPerZoomLevel = ImmutableDictionary<ZoomLevel, TimeSpan>.Empty
                    .Add(zoomLevel, evictionPeriod));

            using var testDb = new TestDatabase(this.outputHelper);

            using (var ctx = await testDb.CreateContextAsync().ConfigureAwait(true))
            {
                ctx.MapImages.Add(
                    new MapImage
                    {
                        ImageData = new byte[] { 0x01 },
                        LastTimeAccessed = imageAccessTime,
                        PostalCode = postalCode,
                        ZoomLevel = zoomLevel,
                    });
                await ctx.SaveChangesAsync().ConfigureAwait(true);
            }

            using (var ctx = await testDb.CreateContextAsync().ConfigureAwait(true))
            {
                var task = new GeoMapImageCacheCleaningTask(
                    ctx,
                    optionsMock.Object,
                    LoggingTestHelper.CreateLogger<GeoMapImageCacheCleaningTask>(this.outputHelper));

                await task.RunAsync(default).ConfigureAwait(true);
            }

            using (var ctx = await testDb.CreateContextAsync().ConfigureAwait(true))
            {
                (await ctx.MapImages.CountAsync().ConfigureAwait(true))
                    .Should()
                    .Be(
                        0,
                        because: "the only image in the cache should have been evicted.");
            }
        }
    }
}
