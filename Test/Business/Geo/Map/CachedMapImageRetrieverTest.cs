// <copyright file="CachedMapImageRetrieverTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Business.Geo.Map
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.Business.Geo.Map.Abstraction;
    using Fvect.Backend.Business.Geo.Map.Implementation;
    using Fvect.Backend.Common.Models.Geo;
    using Fvect.Backend.Data.Database.Model;
    using Moq;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Represents a test for the <see cref="CachedMapImageRetriever"/> class.
    /// </summary>
    public class CachedMapImageRetrieverTest
    {
        private readonly ITestOutputHelper outputHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedMapImageRetrieverTest"/> class.
        /// </summary>
        /// <param name="outputHelper">The test output helper to use.</param>
        public CachedMapImageRetrieverTest(
            ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
        }

        /// <summary>
        /// Tests that when an image is cachable and already within the cache,
        /// the image will be retrieved from the cache and the inner handler will not be called.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous test execution.</returns>
        [Fact]
        public async Task CachableImageWithinCacheRetrievesFromCache()
        {
            using var db = new TestDatabase(this.outputHelper);

            var zipCode = "1234 AB";
            var zipCodeNormalized = "1234";
            var houseNumber = 1;
            var zoomLevel = ZoomLevel.CITY;
            var imageData = new byte[] { 0, 1, 2, 3, };

            var optionsMock = OptionsTestHelper.CreateBackendOptionsMock(
                x => x.Geo.CacheTimesPerZoomLevel = ImmutableDictionary<ZoomLevel, TimeSpan>.Empty.Add(
                    zoomLevel,
                    TimeSpan.MaxValue));

            var innerMock = new Mock<IMapImageRetriever>();

            var logger = LoggingTestHelper.CreateLogger<CachedMapImageRetriever>(this.outputHelper);

            var imageInCache = new MapImage
            {
                PostalCode = zipCodeNormalized,
                ImageData = imageData,
                ZoomLevel = zoomLevel,
                LastTimeAccessed = DateTimeOffset.UtcNow,
            };

            using (var ctx = await db.CreateContextAsync().ConfigureAwait(true))
            {
                ctx.MapImages.Add(imageInCache);
                await ctx.SaveChangesAsync().ConfigureAwait(true);
            }

            Stream? imageStream = default;

            using (var ctx = await db.CreateContextAsync().ConfigureAwait(true))
            {
                var retriever = new CachedMapImageRetriever(
                    ctx,
                    innerMock.Object,
                    optionsMock.Object,
                    logger);

                imageStream = await retriever.GetMapImageAsync(
                    zipCode,
                    houseNumber,
                    zoomLevel,
                    default)
                    .ConfigureAwait(true);
            }

            imageStream
                .Should()
                .NotBeNull(
                    because: "the value should have been retrieved from the cache");

            using var ms = new MemoryStream();
            imageStream!.Position = 0;
            await imageStream.CopyToAsync(ms).ConfigureAwait(true);

            ms.ToArray()
                .Should()
                .BeEquivalentTo(
                    imageData,
                    because: "the retrieved value should be equal to the data intially stored in the cache");

            innerMock.VerifyNoOtherCalls();
        }
    }
}
