// <copyright file="GeoManagerTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Business.Manager
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.Business.Geo.Map.Abstraction;
    using Fvect.Backend.Business.Manager.Implementation;
    using Fvect.Backend.Common.Models.Geo;
    using Moq;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Contains test for the <see cref="GeoManager"/> class.
    /// </summary>
    public class GeoManagerTest
    {
        private readonly ITestOutputHelper outputHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoManagerTest"/> class.
        /// </summary>
        /// <param name="outputHelper">
        /// The test output helper.
        /// </param>
        public GeoManagerTest(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
        }

        /// <summary>
        /// Tests the <see cref="GeoManager.GetMapImageAsync"/> function with default values for height and width.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous testing operation.</returns>
        [Fact]
        public Task MapRetrievalTestDefaultSizeValuesReturnsDefaultSizeFromOptions()
        {
            var opt = OptionsTestHelper.CreateValidOptions();

            return this.DoHappyFlowTest(
                opt.Geo.DefaultResponseHeightForMapImage,
                opt.Geo.DefaultResponseWidthForMapImage,
                null,
                null);
        }

        /// <summary>
        /// Tests the <see cref="GeoManager.GetMapImageAsync"/> function with custom values for height and width.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous testing operation.</returns>
        [Fact]
        public Task MapRetrievalTestCustomSizeValuesReturnsCustomSizeFromOptions()
        {
            var height = 100;
            var width = 200;

            return this.DoHappyFlowTest(
                height,
                width,
                height,
                width);
        }

        /// <summary>
        /// Tests that when the <see cref="IMapImageRetriever"/> returns <c>null</c>, the manager
        /// does not call the <see cref="IImageResizingStrategy"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous testing operation.</returns>
        [Fact]
        public async Task RetrieverReturnsNullManagerReturnsNullWithoutCallingResizingStrategy()
        {
            var imageRetrieverMock = new Mock<IMapImageRetriever>();
            var imageResizerMock = new Mock<IImageResizingStrategy>();
            var optionsMock = OptionsTestHelper.CreateBackendOptionsMock();
            var logger = LoggingTestHelper.CreateLogger<GeoManager>(this.outputHelper);

            var manager = new GeoManager(imageRetrieverMock.Object, imageResizerMock.Object, optionsMock.Object, logger);

            var zipCode = "1234AB";
            var houseNumber = 1;
            var zoomLevel = ZoomLevel.CITY;

            imageRetrieverMock.Setup(x => x.GetMapImageAsync(
                zipCode,
                houseNumber,
                zoomLevel,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Stream)
                .Verifiable();

            var imageStream = await manager.GetMapImageAsync(
                zipCode,
                houseNumber,
                zoomLevel,
                default,
                default,
                default)
                .ConfigureAwait(true);

            imageRetrieverMock.Verify();
            imageRetrieverMock.VerifyNoOtherCalls();
            imageResizerMock.VerifyNoOtherCalls();

            imageStream.Should().BeNull(because: "the manager has not retrieved a value");
        }

        private async Task DoHappyFlowTest(int expectedHeight, int expectedWidth, int? requestedHeight, int? requestedWidth)
        {
            var imageRetrieverMock = new Mock<IMapImageRetriever>();
            var imageResizerMock = new Mock<IImageResizingStrategy>();
            var optionsMock = OptionsTestHelper.CreateBackendOptionsMock();
            var logger = LoggingTestHelper.CreateLogger<GeoManager>(this.outputHelper);

            var manager = new GeoManager(imageRetrieverMock.Object, imageResizerMock.Object, optionsMock.Object, logger);

            var zipCode = "1234AB";
            var houseNumber = 1;
            var zoomLevel = ZoomLevel.CITY;

            using var streamFromRetriever = new MemoryStream();

            imageRetrieverMock.Setup(x => x.GetMapImageAsync(
                zipCode,
                houseNumber,
                zoomLevel,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(streamFromRetriever)
                .Verifiable();

            var imageStream = await manager.GetMapImageAsync(
                zipCode,
                houseNumber,
                zoomLevel,
                requestedHeight,
                requestedWidth,
                default)
                .ConfigureAwait(true);

            imageRetrieverMock.Verify();
            imageRetrieverMock.VerifyNoOtherCalls();

            imageResizerMock.Verify(
                x => x.ResizeJPEGImage(
                    It.Is<MemoryStream>(ms => object.ReferenceEquals(ms, streamFromRetriever)),
                    It.IsAny<Stream>(),
                    expectedHeight,
                    expectedWidth),
                Times.Once);

            imageResizerMock.VerifyNoOtherCalls();

            imageStream.Should().NotBeNull(because: "the manager must have retrieved a value");
        }
    }
}
