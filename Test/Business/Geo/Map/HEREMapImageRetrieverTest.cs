// <copyright file="HEREMapImageRetrieverTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Business.Geo.Map
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.Business.Geo.Map.Implementation;
    using Fvect.Backend.Common.Models.Geo;
    using Fvect.Backend.Data.HERE.Abstraction;
    using Fvect.Backend.Data.HERE.Models;
    using Moq;
    using Xunit;

    /// <summary>
    /// Contains tests for the <see cref="HEREMapImageRetriever"/> test.
    /// </summary>
    public class HEREMapImageRetrieverTest
    {
        private static readonly string PostalCode = "1234AB";
        private static readonly int HouseNumber = 1;
        private static readonly ZoomLevel ZoomLevel = ZoomLevel.CITY;
        private static readonly int ZoomLevelInt = 14;
        private static readonly Position Position = new Position() { Latitude = 12, Longitude = 12, };
        private static readonly int HereMaxHeight = 1152;
        private static readonly int HereMaxWidth = 2048;

        /// <summary>
        /// Tests the mapping of <see cref="ZoomLevel"/> values to values understood by the HERE API.
        /// </summary>
        /// <param name="zoomLevel">The zoom level.</param>
        /// <param name="expectedHEREValue">The expected HERE value.</param>
        [Theory]
        [InlineData(ZoomLevel.HOUSE, 20)]
        [InlineData(ZoomLevel.STREET, 18)]
        [InlineData(ZoomLevel.NEIGHBOURHOOD, 16)]
        [InlineData(ZoomLevel.CITY, 14)]
        [InlineData(ZoomLevel.MUNICIPALITY, 13)]
        [InlineData(ZoomLevel.PROVINCE, 10)]
        public void ZoomLevelToHEREMappingTheory(
            ZoomLevel zoomLevel,
            int expectedHEREValue)
        {
            HEREMapImageRetriever.ZoomLevelToHereValue(zoomLevel)
                .Should()
                .Be(
                    expectedHEREValue,
                    because: "the theory says so");
        }

        /// <summary>
        /// Tests that the <see cref="HEREMapImageRetriever.GetMapImageAsync"/> returns a map
        /// when both Geo info and a map image is available.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task GetMapImageTestGeoInfoAvailableMapAvailableReturnsMap()
        {
            var (retriever, geocodeMock, mapImageMock) = CreateRetrieverWithMocks();
            using var ms = new MemoryStream();

            geocodeMock.Setup(x => x.GetGeoCodeInfoForDutchPostalCodeAndHouseNumber(
                PostalCode, HouseNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GeocodeInfo() { Position = Position, })
                .Verifiable();

            mapImageMock.Setup(x => x.GetMapImage(
                Position, ZoomLevelInt, HereMaxHeight, HereMaxWidth, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ms)
                .Verifiable();

            var image = await retriever.GetMapImageAsync(
                PostalCode,
                HouseNumber,
                ZoomLevel,
                default).ConfigureAwait(true);

            image.Should().BeSameAs(
                ms,
                because: "this was the mocked value");

            geocodeMock.Verify();
            geocodeMock.VerifyNoOtherCalls();
            mapImageMock.Verify();
            mapImageMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests that the <see cref="HEREMapImageRetriever.GetMapImageAsync"/> returns <c>null</c>
        /// when Geo info is not available.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task GetMapImageTestGeoInfoNotAvailableReturnsNull()
        {
            var (retriever, geocodeMock, mapImageMock) = CreateRetrieverWithMocks();

            geocodeMock.Setup(x => x.GetGeoCodeInfoForDutchPostalCodeAndHouseNumber(
                PostalCode, HouseNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as GeocodeInfo)
                .Verifiable();

            var image = await retriever.GetMapImageAsync(
                PostalCode,
                HouseNumber,
                ZoomLevel,
                default).ConfigureAwait(true);

            image.Should().BeNull(
                because: "no geocode information was available");

            geocodeMock.Verify();
            geocodeMock.VerifyNoOtherCalls();
            mapImageMock.VerifyNoOtherCalls();
        }

        private static (HEREMapImageRetriever retriever, Mock<IGeocodeClient> geocodeMock, Mock<IMapImageClient> mapImageMock)
            CreateRetrieverWithMocks()
        {
            var geocodeMock = new Mock<IGeocodeClient>();
            var mapImageMock = new Mock<IMapImageClient>();

            return (
                new HEREMapImageRetriever(
                    geocodeMock.Object,
                    mapImageMock.Object),
                geocodeMock,
                mapImageMock);
        }
    }
}
