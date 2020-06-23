// <copyright file="MapImageClientTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Data.HERE
{
    using System;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.Common.Exception;
    using Fvect.Backend.Common.Models.Geo;
    using Fvect.Backend.Data.HERE.Implementation;
    using Moq;
    using Xunit;

    /// <summary>
    /// Contains tests for the <see cref="MapImageClient"/> class.
    /// </summary>
    public class MapImageClientTest
    {
        private static readonly Uri BaseUri = new Uri("https://example.com/");
        private static readonly string ApiKey = "ABCD";
        private static readonly Position Position = new Position() { Latitude = 2, Longitude = 3, };
        private static readonly int ZoomLevel = 2;
        private static readonly int RequestedHeight = 200;
        private static readonly int RequestedWidth = 300;

        /// <summary>
        /// Tests that retrieving geocode information where an HTTP error occurs results in a <see cref="DataProviderException"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task RetrieveGeoCodeInfoExceptionHttpExceptionRethrownAsDataProviderException()
        {
            var (client, mock) = CreateClientAndHandlerMock(
                mock => mock.SetupHttpGet(CheckUri)
                .ThrowsAsync(new HttpRequestException(string.Empty, new Exception()))
                .Verifiable());

            Func<Task> testCode = async () => await client.GetMapImage(
                Position,
                ZoomLevel,
                RequestedHeight,
                RequestedWidth,
                default)
                .ConfigureAwait(true);

            await testCode
                .Should()
                .ThrowAsync<DataProviderException>(
                    because: "an HTTP error was simulated")
                .ConfigureAwait(true);

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests the happy flow of retrieving Geocode Information.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task RetrieveGeoCodeInfoCorrectResponseReturnsCorrectValue()
        {
            using var hashAlgo = new SHA512Managed();
            using var responseContent = new FileStream(
                Path.Combine("Assets", "HEREMap.jpg"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var responseContentHash = hashAlgo.ComputeHash(responseContent);
            responseContent.Position = 0;

            var (client, mock) = CreateClientAndHandlerMock(
                mock => mock.SetupHttpGet(CheckUri)
                .ReturnsStreamBodyAsync(responseContent)
                .Verifiable());

            var mapImageStream = await client.GetMapImage(
                Position,
                ZoomLevel,
                RequestedHeight,
                RequestedWidth,
                default)
                .ConfigureAwait(true);

            var retrievedContentHash = hashAlgo.ComputeHash(mapImageStream);

            retrievedContentHash.Should()
                .BeEquivalentTo(
                    responseContentHash,
                    because: "A successfull response was simulated.");

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests the case when a 500 response is returned from the data source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task RetrieveMapInfo500ResponseThrowsDataProviderException()
        {
            var (client, mock) = CreateClientAndHandlerMock(
                mock => mock.SetupHttpGet(CheckUri)
                .ReturnsStringBodyAsync("ERROR", HttpStatusCode.InternalServerError)
                .Verifiable());

            Func<Task> testCode = async () => await client.GetMapImage(
                Position,
                ZoomLevel,
                RequestedHeight,
                RequestedWidth,
                default)
                .ConfigureAwait(false);

            await testCode
                .Should()
                .ThrowAsync<DataProviderException>(
                    because: "a server error was simulated")
                .ConfigureAwait(true);

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        private static (MapImageClient client, Mock<HttpMessageHandler> httpMock) CreateClientAndHandlerMock(
            Action<Mock<HttpMessageHandler>>? mockSetup = null)
        {
            var (httpMock, httpClient) = HttpTestHelper.CreateMockWithClient(mockSetup);
            var optionsMock = OptionsTestHelper.CreateBackendOptionsMock(
                x =>
                {
                    x.Geo.HEREMapImageServiceBaseUri = BaseUri;
                    x.Geo.HereMapsAPIKey = ApiKey;
                });

            return (new MapImageClient(httpClient, optionsMock.Object), httpMock);
        }

        private static bool CheckUri(Uri uri) => HttpTestHelper.CheckUri(
            uri,
            Uri.UriSchemeHttps,
            BaseUri.Host,
            "/mia/1.6/mapview",
            ImmutableDictionary<string, string>.Empty
                .Add("apiKey", ApiKey)
                .Add("ml", "dut")
                .Add("f", "1")
                .Add("ppi", "250")
                .Add("nodot", "true")
                .Add("c", Position.ToString())
                .Add("z", ZoomLevel.ToString(CultureInfo.InvariantCulture))
                .Add("h", RequestedHeight.ToString(CultureInfo.InvariantCulture))
                .Add("w", RequestedWidth.ToString(CultureInfo.InvariantCulture)));
    }
}
