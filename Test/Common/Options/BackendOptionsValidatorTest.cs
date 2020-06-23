// <copyright file="BackendOptionsValidatorTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Common.Options
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Fvect.Backend.Common.Options;
    using Xunit;

    /// <summary>
    /// Contains tests for the <see cref="BackendOptionsValidator"/> class.
    /// </summary>
    public class BackendOptionsValidatorTest
    {
        /// <summary>
        /// Tests that the validator does not produce any
        /// faults when validating a valid options instance.
        /// </summary>
        [Fact]
        public void DoesNotProduceFaultsForValidOptions()
        {
            var validator = new BackendOptionsValidator();
            var options = OptionsTestHelper.CreateValidOptions();
            var result = validator.Validate(string.Empty, options);
            result.Failed.Should().BeFalse("because the provided options are valid");
        }

        /// <summary>
        /// Tests that the validator produces at least one fault
        /// when the SQL Server connection string is empty.
        /// </summary>
        [Fact]
        public void DoesProduceFaultForEmptySqlServerConnectionString()
        {
            var validator = new BackendOptionsValidator();
            var options = OptionsTestHelper.CreateValidOptions();
            options.Database.SQLServerConnectionString = string.Empty;

            var result = validator.Validate(string.Empty, options);

            result.Failed.Should().BeTrue("because the provided options are not valid");
        }

        /// <summary>
        /// Tests that the validator produces at least one fault
        /// when the Here Maps API Key is not empty.
        /// </summary>
        [Fact]
        public void DoesProduceFaultForEmptyHereMapsApiKey()
        {
            var validator = new BackendOptionsValidator();
            var options = OptionsTestHelper.CreateValidOptions();
            options.Geo.HereMapsAPIKey = string.Empty;

            var result = validator.Validate(string.Empty, options);

            result.Failed.Should().BeTrue("because the provided options are not valid.");
        }

        /// <summary>
        /// Tests that the validator produces at least one fault
        /// when the HERE Geocode Service endpoint has an invalid scheme.
        /// </summary>
        [Fact]
        public void DoesProduceFaultForHEREGeocodeServiceEndpointWithInvalidScheme()
        {
            var validator = new BackendOptionsValidator();
            var options = OptionsTestHelper.CreateValidOptions();

            var uriBuilder = new UriBuilder("https://google.com/")
            {
                Scheme = Uri.UriSchemeFtp,
            };
            options.Geo.HEREGeocodeServiceBaseUri = uriBuilder.Uri;

            var result = validator.Validate(string.Empty, options);

            result.Failed.Should()
                .BeTrue(
                    because: "the HERE Geocode Service Endpoint has an invalid URI.");
        }

        /// <summary>
        /// Tests that the validator produces at least one fault
        /// when the HERE Map Image endpoint is <c>null</c>.
        /// </summary>
        [Fact]
        public void DoesProduceFaultForHEREMapImageServiceEndpointNull()
        {
            var validator = new BackendOptionsValidator();
            var options = OptionsTestHelper.CreateValidOptions();

            options.Geo.HEREMapImageServiceBaseUri = null!;

            var result = validator.Validate(string.Empty, options);

            result.Failed.Should()
                .BeTrue(
                    because: "the HERE Map Image service endpoint Uri is null.");
        }

        /// <summary>
        /// Tests that the validator produces at least one fault
        /// when the JWT signing key is not at least 16 characters.
        /// </summary>
        [Fact]
        public void DoesProduceFaultForTooShortJwtSecret()
        {
            var validator = new BackendOptionsValidator();
            var options = OptionsTestHelper.CreateValidOptions();

            options.AuthNR.JWTSigningKey = string.Concat(
                Enumerable.Range(0, 15).Select(n => "c"));

            var result = validator.Validate(string.Empty, options);

            result.Failed.Should()
                .BeTrue(
                    because: "the JWT signing key was not at least 16 characters long.");
        }
    }
}
