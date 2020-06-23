// <copyright file="Teacher.cs" company="Fvect">
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
    [Table("Teacher")]
    public class Teacher : ResourceEntityBase<Teacher>
    {
        /// <summary>
        /// Gets or sets the identifier of the user related to this
        /// entity.
        /// </summary>
        public Guid? AppUserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the teacher.
        /// </summary>
        [Required]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the student groups of the teacher.
        /// </summary>
        [ForeignKey("TeacherId")]
        public IEnumerable<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();

        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            base.Configure(builder);

            builder
                .HasOne<AppUser>()
                .WithOne()
                .HasForeignKey<Teacher>(x => x.AppUserId);
        }
    }
}
