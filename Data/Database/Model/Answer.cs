// <copyright file="Answer.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics.CodeAnalysis;
    using Fvect.Backend.Data.Database.Base;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Represents an Answer.
    /// </summary>
    [Table("Answer")]
    public class Answer : ResourceEntityBase<Answer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Answer"/> class.
        /// </summary>
        public Answer()
        {
            this.Value = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Answer"/> class.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="Answer"/>.</param>
        public Answer(Guid id)
            : this()
        {
            this.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Answer"/> class.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="Answer"/>.</param>
        /// <param name="value">The value of the <see cref="Answer"/> itself.</param>
        /// <param name="questionId">The identifier of the <see cref="Question"/>.</param>
        /// <param name="correct">Whether or not the <see cref="Answer"/> is correct.</param>
        public Answer(Guid id, string value, Guid questionId, bool correct)
        {
            this.Id = id;
            this.Value = value;
            this.QuestionId = questionId;
            this.Correct = correct;
        }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the question id.
        /// </summary>
        [ForeignKey("Question")]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the answer is correct.
        /// </summary>
        public bool Correct { get; set; }

        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            base.Configure(builder);

            // Add unique on the answer
            builder.HasIndex(answer => answer.Value)
                .HasName("UX_Answer_Value")
                .IsUnique();
        }
    }
}
