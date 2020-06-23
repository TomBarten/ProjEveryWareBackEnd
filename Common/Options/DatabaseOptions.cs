// <copyright file="DatabaseOptions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Options
{
    /// <summary>
    /// Represents the database part of the application options.
    /// </summary>
    /// <remarks>
    /// The non-nullable properties of this class are always set to null.
    /// This is not a problem because the <see cref="BackendOptionsValidator"/>
    /// checks that these values are not null.
    /// </remarks>
    public class DatabaseOptions
    {
        /// <summary>
        /// Gets or sets the SQL Server Connection string to use.
        /// </summary>
        public string SQLServerConnectionString { get; set; } = null!;
    }
}
