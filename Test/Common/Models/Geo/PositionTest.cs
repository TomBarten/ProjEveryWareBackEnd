// <copyright file="PositionTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Common.Models.Geo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Fvect.Backend.Common.Models.Geo;
    using Xunit;

    /// <summary>
    /// Contains tests for the <see cref="Position"/> type.
    /// </summary>
    public class PositionTest
    {
        /// <summary>
        /// Tests the equality functionality of the <see cref="Position"/> struct.
        /// </summary>
        /// <param name="a">Postion a.</param>
        /// <param name="b">Position b.</param>
        /// <param name="areEqual">A value indicating whether <paramref name="a"/> and <paramref name="b"/> are equal.</param>
        [Theory]
        [MemberData(nameof(EqualityTheoryData))]
        public void EqualityTheory(
            Position a,
            Position b,
            bool areEqual)
        {
            (a == b)
                .Should()
                .Be(areEqual);

            (a != b)
                .Should()
                .Be(!areEqual);

            a.Equals(b)
                .Should()
                .Be(areEqual);

            a.Equals((object)b)
                .Should()
                .Be(areEqual);
        }

        /// <summary>
        /// Tests the hashcode functionality of the <see cref="Position"/> struct.
        /// </summary>
        [Fact]
        [SuppressMessage("Security", "SCS0005:Weak random generator", Justification = "Not used for cryptography.")]
        public void HashCodeTest()
        {
            var rnd = new Random();
            var lat = rnd.NextDouble();
            var @long = rnd.NextDouble();

            var pos = new Position() { Latitude = lat, Longitude = @long, };

            pos.GetHashCode()
                .Should()
                .Be(
                    HashCode.Combine(
                        lat.GetHashCode(),
                        @long.GetHashCode()));
        }

        private static IEnumerable<object[]> EqualityTheoryData()
        {
            yield return new object[] { new Position() { Latitude = 1, Longitude = 2.3 }, new Position() { Latitude = 1, Longitude = 2.3 }, true };
            yield return new object[] { new Position() { Latitude = 1.2, Longitude = 13 }, new Position() { Latitude = 1.2, Longitude = 13 }, true };
            yield return new object[] { new Position() { Latitude = 1.2, Longitude = 13 }, new Position() { Latitude = 1.1, Longitude = 13 }, false };
            yield return new object[] { new Position() { Latitude = 1.2, Longitude = 13 }, new Position() { Latitude = 1.2, Longitude = 12 }, false };
            yield return new object[] { new Position() { Latitude = 1.2, Longitude = 13 }, new Position() { Latitude = -1, Longitude = 3 }, false };
        }
    }
}
