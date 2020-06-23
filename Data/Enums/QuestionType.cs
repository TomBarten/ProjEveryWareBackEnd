// <copyright file="QuestionType.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents the question type.
    /// </summary>
    public enum QuestionType
    {
        /// <summary>
        /// Means the question has no set type.
        /// </summary>
        Defaulted = 0,

        /// <summary>
        /// Means the question is open for any answer.
        /// </summary>
        Open = 1,

        /// <summary>
        /// Means the question has predefined answers that can be given.
        /// </summary>
        Multiple = 2,
    }
}
