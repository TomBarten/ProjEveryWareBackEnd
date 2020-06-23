// <copyright file="Level.cs" company="Fvect">
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
    using System.Diagnostics.CodeAnalysis;
    using Fvect.Backend.Data.Database.Base;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Represents a Level.
    /// </summary>
    [Table("Level")]
    public class Level : ResourceEntityBase<Level>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        public Level()
        {
            this.Questions = new List<Question>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        /// <param name="number">The number of the <see cref="Level"/>.</param>
        public Level(int number)
            : this()
        {
            this.Number = number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        /// <param name="number">The number of the <see cref="Level"/>.</param>
        /// <param name="questions">A collection of <see cref="Question"/> that belong to this <see cref="Level"/>.</param>
        public Level(int number, IEnumerable<Question> questions)
        {
            this.Number = number;
            this.Questions = questions;
        }

        /// <summary>
        /// Gets or sets the <see cref="Level"/> number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Level"/> questions.
        /// </summary>
        [ForeignKey("Level")]
        public IEnumerable<Question> Questions { get; set; }

        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<Level> builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            base.Configure(builder);

            // Add unique on the level number
            builder.HasIndex(level => level.Number)
                .HasName("UX_Level_Number")
                .IsUnique();
        }
    }
}
