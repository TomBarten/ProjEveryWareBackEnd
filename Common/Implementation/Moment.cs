// <copyright file="Moment.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Implementation
{
    using System;
    using Fvect.Backend.Common.Abstraction;

    /// <summary>
    /// Impl. for <see cref="IMoment" />.
    /// </summary>
    public class Moment : IMoment
    {
        /// <inheritdoc />
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
