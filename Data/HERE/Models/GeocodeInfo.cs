// <copyright file="GeocodeInfo.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.HERE.Models
{
    using Fvect.Backend.Common.Models.Geo;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents geocoding information.
    /// </summary>
    public class GeocodeInfo
    {
        /// <summary>
        /// Gets or sets the title of the geocode subject.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the identifier used by HERE for the geocode subject.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Gets or sets the address information of the geocode subject.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Address Address { get; set; } = null!;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Position Position { get; set; }
    }
}
