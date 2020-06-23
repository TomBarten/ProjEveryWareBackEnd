// <copyright file="Question.cs" company="Fvect">
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
    using Fvect.Backend.Data.Enums;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents a Question.
    /// </summary>
    [Table("Question")]
    public class Question : ResourceEntityBase<Question>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Question"/> class.
        /// </summary>
        public Question()
        {
            this.Value = string.Empty;
            this.Answers = new List<Answer>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Question"/> class.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="Question"/>.</param>
        public Question(Guid id)
            : this()
        {
            this.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Question"/> class.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="Question"/>.</param>
        /// <param name="value">The value of the <see cref="Question"/> itself.</param>
        /// <param name="level">The <see cref="Model.Level"/> to which this <seealso cref="Question"/> belongs to.</param>
        /// <param name="questionType">The <see cref="Enums.QuestionType"/> of the <see cref="Question"/>.</param>
        /// <param name="answers">A collection of <see cref="Answer"/> that belong to this <seealso cref="Question"/>.</param>
        public Question(Guid id, string value, Guid level, QuestionType questionType, IEnumerable<Answer> answers)
        {
            this.Id = id;
            this.Value = value;
            this.Level = level;
            this.QuestionType = questionType;
            this.Answers = answers;
        }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the level number.
        /// </summary>
        [ForeignKey("Level")]
        public Guid Level { get; set; }

        /// <summary>
        /// Gets or sets the question type.
        /// </summary>
        public QuestionType QuestionType { get; set; }

        /// <summary>
        /// Gets or sets the possible answers.
        /// </summary>
        [ForeignKey("QuestionId")]
        public IEnumerable<Answer> Answers { get; set; }
    }
}
