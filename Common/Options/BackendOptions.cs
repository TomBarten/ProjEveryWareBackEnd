// <copyright file="BackendOptions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Options
{
    /// <summary>
    /// Represents the application configuration.
    /// </summary>
    /// <remarks>
    /// The non-nullable properties of this class are always set to null.
    /// This is not a problem because the <see cref="BackendOptionsValidator"/>
    /// checks that these values are not null.
    /// </remarks>
    public class BackendOptions
    {
        /// <summary>
        /// Gets or sets the database portion of the application options.
        /// </summary>
        public DatabaseOptions Database { get; set; } = null!;

        /// <summary>
        /// Gets or sets the geographic services portion of the applications options.
        /// </summary>
        public GeoOptions Geo { get; set; } = null!;

        /// <summary>
        /// Gets or sets the options for authentication and authorization.
        /// </summary>
        public AuthNROptions AuthNR { get; set; } = null!;
    }
}
