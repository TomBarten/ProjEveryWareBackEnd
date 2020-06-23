// <copyright file="Address.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.HERE.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an address as returned by the Geocode API.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the ISO 3166 ALPHA-3 Country Code of the address.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string CountryCode { get; set; } = null!;

        /// <summary>
        /// Gets or sets the state of the address.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string State { get; set; } = null!;

        /// <summary>
        /// Gets or sets the county of the address.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string County { get; set; } = null!;

        /// <summary>
        /// Gets or sets the city of the address.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string City { get; set; } = null!;

        /// <summary>
        /// Gets or sets the postal code of the address.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string PostalCode { get; set; } = null!;

        /// <summary>
        /// Gets or sets the street of the address. May be <c>null</c>.
        /// </summary>
        public string? Street { get; set; }

        /// <summary>
        /// Gets or sets the house number of the address. May be <c>null</c>.
        /// </summary>
        public int? HouseNumber { get; set; }
    }
}
