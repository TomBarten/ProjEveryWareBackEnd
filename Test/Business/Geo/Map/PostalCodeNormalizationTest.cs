// <copyright file="PostalCodeNormalizationTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Business.Geo.Map
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using FluentAssertions;
    using Fvect.Backend.Business.Geo.Map;
    using Fvect.Backend.Common.Models.Geo;
    using Xunit;
    using static Fvect.Backend.Business.Geo.Map.PostalCodeNormalization;

    /// <summary>
    /// Provides tests for the <see cref="PostalCodeNormalization"/> class.
    /// </summary>
    public class PostalCodeNormalizationTest
    {
        /// <summary>
        /// Tests various postal code normalization cases.
        /// </summary>
        /// <param name="postalCodeInput">The input postal code.</param>
        /// <param name="zoomLevel">The zoom level.</param>
        /// <param name="expectedOutput">The expected output.</param>
        [Theory]
        [MemberData(nameof(TestCases))]
        public void PostalCodeNormalizationTheory(
            string postalCodeInput,
            ZoomLevel zoomLevel,
            string expectedOutput)
        {
            var normalized = NormalizeDutchPostalCode(
                postalCodeInput,
                zoomLevel);

            normalized
                .Should()
                .Be(expectedOutput);
        }

        private static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "1234AB", ZoomLevel.HOUSE, "1234AB" };
            yield return new object[] { "1234 AB", ZoomLevel.STREET, "1234AB" };
            yield return new object[] { "1234 AB", ZoomLevel.CITY, "1234" };
        }
    }
}
