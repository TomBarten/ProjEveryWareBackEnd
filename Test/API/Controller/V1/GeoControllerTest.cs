// <copyright file="GeoControllerTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.Controller.V1
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.API.V1.Controllers;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Common.Models.Geo;
    using Fvect.Backend.Common.Options;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    /// <summary>
    /// Contains tests for the <see cref="GeoController"/> class.
    /// </summary>
    public class GeoControllerTest
    {
        private static readonly string ZipCode = "1234AB";
        private static readonly int HouseNumber = 2;

        /// <summary>
        /// Tests attempting to retrieve a map image with default height and width
        /// results in a map being returned.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task GetMapImageHeightWidthDefaultValuesReturnsMap()
        {
            var (controller, managerMock, options) = CreateControllerWithMocksAndOptions();

            using var returnStream = new MemoryStream();

            managerMock.Setup(x => x.GetMapImageAsync(
                ZipCode,
                HouseNumber,
                ZoomLevel.CITY,
                default,
                default,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnStream)
                .Verifiable();

            var response = await controller.GetMapImage(
                ZipCode,
                HouseNumber,
                ZoomLevel.CITY,
                default,
                default,
                default)
                .ConfigureAwait(true);

            (response as FileStreamResult) !.FileStream.Should().BeSameAs(returnStream);

            managerMock.Verify();
            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests attempting to retrieve a map image with custom height and width
        /// results in a map being returned.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task GetMapImageHeightWidthCustomValuesWithinRangeReturnsMap()
        {
            var (controller, managerMock, options) = CreateControllerWithMocksAndOptions();

            using var returnStream = new MemoryStream();

            managerMock.Setup(x => x.GetMapImageAsync(
                ZipCode,
                HouseNumber,
                ZoomLevel.CITY,
                options.Geo.MaxRequestHeightForMapImage,
                options.Geo.MaxRequestWidthForMapImage,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnStream)
                .Verifiable();

            var response = await controller.GetMapImage(
                ZipCode,
                HouseNumber,
                ZoomLevel.CITY,
                default,
                options.Geo.MaxRequestHeightForMapImage,
                options.Geo.MaxRequestWidthForMapImage)
                .ConfigureAwait(true);

            (response as FileStreamResult) !.FileStream.Should().BeSameAs(returnStream);

            managerMock.Verify();
            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests attempting to retrieve a map image with an invalid
        /// house number results in a bad request response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task GetMapImageHeightWidthDefaultValuesHouseNumberInvalidReturnsBadRequest()
        {
            var (controller, managerMock, _) = CreateControllerWithMocksAndOptions();

            var response = await controller.GetMapImage(
                ZipCode,
                -1,
                ZoomLevel.CITY,
                default,
                default,
                default)
                .ConfigureAwait(true);

            response.Should().BeOfType<BadRequestObjectResult>(
                because: "the house number was out of range");

            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests attempting to retrieve a map image with an invalid height
        /// results in a bad request response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task GetMapImageHeightWidthCustomValuesHeightOutOfRangeReturnsBadRequest()
        {
            var (controller, managerMock, options) = CreateControllerWithMocksAndOptions();

            var response = await controller.GetMapImage(
                ZipCode,
                HouseNumber,
                ZoomLevel.CITY,
                default,
                options.Geo.MaxRequestHeightForMapImage + 1,
                options.Geo.MaxRequestWidthForMapImage)
                .ConfigureAwait(true);

            response.Should().BeOfType<BadRequestObjectResult>(
                because: "the height was out of range");

            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests attempting to retrieve a map image with an invalid width
        /// results in a bad request response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task GetMapImageHeightWidthCustomValuesWidthOutOfRangeReturnsBadRequest()
        {
            var (controller, managerMock, options) = CreateControllerWithMocksAndOptions();

            var response = await controller.GetMapImage(
                ZipCode,
                HouseNumber,
                ZoomLevel.CITY,
                default,
                options.Geo.MaxRequestHeightForMapImage,
                options.Geo.MaxRequestWidthForMapImage + 1)
                .ConfigureAwait(true);

            response.Should().BeOfType<BadRequestObjectResult>(
                because: "the width was out of range");

            managerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests attempting to retrieve a map image with an invalid zip code
        /// results in a bad request response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task GetMapImageHeightWidthDefaultValuesZipcodeInvalidReturnsBadRequest()
        {
            var (controller, managerMock, _) = CreateControllerWithMocksAndOptions();

            var response = await controller.GetMapImage(
                "122 A",
                HouseNumber,
                ZoomLevel.CITY,
                default,
                default,
                default)
                .ConfigureAwait(true);

            response.Should().BeOfType<BadRequestObjectResult>(
                because: "the zip code was not valid");

            managerMock.VerifyNoOtherCalls();
        }

        private static (
            GeoController controller,
            Mock<IGeoManager> managerMock,
            BackendOptions options) CreateControllerWithMocksAndOptions()
        {
            var managerMock = new Mock<IGeoManager>();
            var optionsMock = OptionsTestHelper.CreateBackendOptionsMock();
            return (
                new GeoController(
                    managerMock.Object,
                    optionsMock.Object),
                managerMock,
                optionsMock.Object.CurrentValue);
        }
    }
}
