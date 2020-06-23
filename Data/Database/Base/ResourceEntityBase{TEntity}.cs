// <copyright file="ResourceEntityBase{TEntity}.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.Base
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Represents an entity with a <see cref="Guid" /> as primary key and
    /// a timestamp for optimistic concurrency.
    /// </summary>
    /// <typeparam name="TEntity">The type of the inheriting entity.</typeparam>
    public abstract class ResourceEntityBase<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : ResourceEntityBase<TEntity>, new()
    {
        /// <summary>
        /// Gets or sets the identifier of this entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the version of the row. This property is
        /// automatically managed by EF Core and is used to prevent
        /// concurrency issues.
        /// </summary>
        [SuppressMessage(
            "Performance",
            "CA1819:Properties should not return arrays",
            Justification = "Required by EF Core.")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Timestamp] // This timestamp attribute needs to be there for the tests to be able to run de models with SQLite.
        public byte[] RowVersion { get; set; } = null!;

        /// <inheritdoc />
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
