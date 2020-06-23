// <copyright file="GeocodeClientTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Data.HERE
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fvect.Backend.Common.Exception;
    using Fvect.Backend.Data.HERE.Implementation;
    using Moq;
    using Xunit;

    /// <summary>
    /// Contains tests for the <see cref="GeocodeClient"/> class.
    /// </summary>
    public class GeocodeClientTest
    {
        private static readonly Uri BaseUri = new Uri("https://example.com/");
        private static readonly string ApiKey = "ABCD";
        private static readonly string PostalCode = "1234AB";
        private static readonly int HouseNumber = 1;

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

            Func<Task> testCode = async () => await client.GetGeoCodeInfoForDutchPostalCodeAndHouseNumber(
                PostalCode,
                HouseNumber,
                default)
                .ConfigureAwait(false);

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
            var responseContent = await File.ReadAllTextAsync(Path.Combine("Assets", "HEREGeocodeFilledResponse.json"))
                .ConfigureAwait(true);

            var (client, mock) = CreateClientAndHandlerMock(
                mock => mock.SetupHttpGet(CheckUri)
                .ReturnsStringBodyAsync(responseContent)
                .Verifiable());

            var geocodeData = await client.GetGeoCodeInfoForDutchPostalCodeAndHouseNumber(
                PostalCode,
                HouseNumber,
                default)
                .ConfigureAwait(true);

            geocodeData.Should()
                .NotBeNull(
                    because: "A successfull response was simulated.");

            geocodeData!.Title.Should().Be("3739 LB, Hollandsche Rading, Utrecht, Nederland");
            geocodeData.Id.Should().Be("here:cm:namedplace:26325864");
            geocodeData.Position.Latitude.Should().Be(52.17428);
            geocodeData.Position.Longitude.Should().Be(5.19736);
            geocodeData.Address.HouseNumber.Should().BeNull();
            geocodeData.Address.PostalCode.Should().Be("3739 LB");
            geocodeData.Address.Street.Should().BeNull();
            geocodeData.Address.State.Should().Be("Utrecht");
            geocodeData.Address.County.Should().Be("De Bilt");
            geocodeData.Address.City.Should().Be("Hollandsche Rading");

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests the case when an empty list is returned from the data source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task RetrieveGeoCodeInfoEmptyResponseReturnsNull()
        {
            var responseContent = await File.ReadAllTextAsync(Path.Combine("Assets", "HEREGeocodeEmptyResponse.json"))
                .ConfigureAwait(true);

            var (client, mock) = CreateClientAndHandlerMock(
                mock => mock.SetupHttpGet(CheckUri)
                .ReturnsStringBodyAsync(responseContent)
                .Verifiable());

            var geocodeData = await client.GetGeoCodeInfoForDutchPostalCodeAndHouseNumber(
                PostalCode,
                HouseNumber,
                default)
                .ConfigureAwait(true);

            geocodeData.Should()
                .BeNull(
                    because: "An empty response was simulated.");

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests the case when a 404 response is returned from the data source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task RetrieveGeoCodeInfo404ResponseReturnsNull()
        {
            var (client, mock) = CreateClientAndHandlerMock(
                mock => mock.SetupHttpGet(CheckUri)
                .ReturnsStringBodyAsync("NOT FOUND", HttpStatusCode.NotFound)
                .Verifiable());

            var geocodeData = await client.GetGeoCodeInfoForDutchPostalCodeAndHouseNumber(
                PostalCode,
                HouseNumber,
                default)
                .ConfigureAwait(true);

            geocodeData.Should()
                .BeNull(
                    because: "An empty response was simulated.");

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests the case when a 500 response is returned from the data source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task RetrieveGeoCodeInfo500ResponseThrowsDataProviderException()
        {
            var (client, mock) = CreateClientAndHandlerMock(
                mock => mock.SetupHttpGet(CheckUri)
                .ReturnsStringBodyAsync("ERROR", HttpStatusCode.InternalServerError)
                .Verifiable());

            Func<Task> testCode = async () => await client.GetGeoCodeInfoForDutchPostalCodeAndHouseNumber(
                PostalCode,
                HouseNumber,
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

        private static (GeocodeClient client, Mock<HttpMessageHandler> httpMock) CreateClientAndHandlerMock(
            Action<Mock<HttpMessageHandler>>? mockSetup = null)
        {
            var (httpMock, httpClient) = HttpTestHelper.CreateMockWithClient(mockSetup);
            var optionsMock = OptionsTestHelper.CreateBackendOptionsMock(
                x =>
                {
                    x.Geo.HEREGeocodeServiceBaseUri = BaseUri;
                    x.Geo.HereMapsAPIKey = ApiKey;
                });

            return (new GeocodeClient(httpClient, optionsMock.Object), httpMock);
        }

        private static bool CheckUri(Uri uri) => HttpTestHelper.CheckUri(
            uri,
            Uri.UriSchemeHttps,
            BaseUri.Host,
            "/v1/geocode",
            ImmutableDictionary<string, string>.Empty
                .Add("apiKey", ApiKey)
                .Add("lang", "nl-NL")
                .Add("in", "countryCode:NLD")
                .Add("qq", $"postalCode={PostalCode};houseNumber={HouseNumber}"));
    }
}
