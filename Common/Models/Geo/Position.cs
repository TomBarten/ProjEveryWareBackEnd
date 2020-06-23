// <copyright file="Position.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Models.Geo
{
    using System;
    using System.Globalization;
    using System.Text.Json.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a geographical position.
    /// </summary>
    public struct Position : IEquatable<Position>
    {
        /// <summary>
        /// Gets or sets the latitude of the location.
        /// </summary>
        [JsonProperty(PropertyName = "lat", Required = Required.Always)]
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the logitude of the location.
        /// </summary>
        [JsonProperty(PropertyName = "lng", Required = Required.Always)]
        [JsonPropertyName("lng")]
        public double Longitude { get; set; }

        /// <summary>
        /// Checks if two positions are equal.
        /// </summary>
        /// <param name="left">The left position.</param>
        /// <param name="right">The right position.</param>
        /// <returns>A value indicating whether the positions are equal.</returns>
        public static bool operator ==(Position left, Position right)
            => left.Equals(right);

        /// <summary>
        /// Checks if two positions are not equal.
        /// </summary>
        /// <param name="left">The left position.</param>
        /// <param name="right">The right position.</param>
        /// <returns>A value indicating whether the positions are not equal.</returns>
        public static bool operator !=(Position left, Position right)
            => !(left == right);

        /// <inheritdoc />
        public override string ToString() =>
            $"{this.Latitude.ToString(CultureInfo.InvariantCulture)},{this.Longitude.ToString(CultureInfo.InvariantCulture)}";

        /// <inheritdoc />
        public bool Equals(Position other)
            => this.Latitude == other.Latitude && this.Longitude == other.Longitude;

        /// <inheritdoc />
        public override bool Equals(object obj)
            => !(obj is null) && (obj is Position pos)
                ? this.Equals(pos)
                : false;

        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(
                this.Latitude.GetHashCode(),
                this.Longitude.GetHashCode());
    }
}
