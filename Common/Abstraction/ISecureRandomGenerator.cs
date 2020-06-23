// <copyright file="ISecureRandomGenerator.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Abstraction
{
    using System;

    /// <summary>
    /// Specifies an object that can generate crytographically secure
    /// random values.
    /// </summary>
    public interface ISecureRandomGenerator
    {
        /// <summary>
        /// Gets a array of bytes of the specified length, filled
        /// with cryptographically random values.
        /// </summary>
        /// <param name="count">The count of bytes.</param>
        /// <returns>The created array of bytes.</returns>
        byte[] GetRandomBytes(int count);
    }
}
