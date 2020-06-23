// <copyright file="MapImage.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using Fvect.Backend.Common.Models.Geo;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Represents a cached map image.
    /// </summary>
    public class MapImage : IEntityTypeConfiguration<MapImage>
    {
        /// <summary>
        /// Gets or sets the postal code of the map image.
        /// May or may not contain letters.
        /// </summary>
        [Required]
        [MinLength(4)]
        [MaxLength(6)]
        public string PostalCode { get; set; } = null!;

        /// <summary>
        /// Gets or sets the zoom level of the map image.
        /// </summary>
        [Required]
        public ZoomLevel ZoomLevel { get; set; }

        /// <summary>
        /// Gets or sets the image data (in JPEG format) of
        /// the map image.
        /// </summary>
        [Required]
        [SuppressMessage(
            "Performance",
            "CA1819:Properties should not return arrays",
            Justification = "Required by EF Core.")]
        public byte[] ImageData { get; set; } = null!;

        /// <summary>
        /// Gets or sets the time (in UTC) that this <see cref="MapImage"/>
        /// entity was last read.
        /// </summary>
        [Required]
        public DateTimeOffset LastTimeAccessed { get; set; }

        /// <summary>
        /// Gets or sets the version of the row. This property is
        /// automatically managed by EF Core and is used to prevent
        /// concurrency issues.
        /// </summary>
        [SuppressMessage(
            "Performance",
            "CA1819:Properties should not return arrays",
            Justification = "Required by EF Core.")]
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<MapImage> entityBuilder)
        {
            entityBuilder = entityBuilder ?? throw new ArgumentNullException(nameof(entityBuilder));
            entityBuilder.HasKey(i => new { i.PostalCode, i.ZoomLevel });
        }
    }
}
