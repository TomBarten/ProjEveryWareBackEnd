// <copyright file="IMoment.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Abstraction
{
    using System;

    /// <summary>
    /// Specifies an object that can provide the current date and time in UTC.
    /// </summary>
    public interface IMoment
    {
        /// <summary>
        /// Gets the current date and time in UTC.
        /// </summary>
        public DateTimeOffset UtcNow { get; }
    }
}
