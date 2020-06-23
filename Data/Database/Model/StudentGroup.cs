// <copyright file="StudentGroup.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using Fvect.Backend.Data.Database.Base;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Represents a teacher.
    /// </summary>
    [Table("StudentGroup")]
    public class StudentGroup : ResourceEntityBase<StudentGroup>
    {
        /// <summary>
        /// Gets or sets the name of the student group.
        /// </summary>
        [Required]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the teacher id.
        /// </summary>
        public Guid? TeacherId { get; set; }

        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<StudentGroup> builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            base.Configure(builder);

            builder
                .HasOne<Teacher>()
                .WithOne()
                .HasForeignKey<StudentGroup>(x => x.TeacherId);
        }
    }
}
