// <copyright file="SecureRandomGenerator.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Implementation
{
    using System;
    using System.Security.Cryptography;
    using Fvect.Backend.Common.Abstraction;

    /// <summary>
    /// Impl. for <see cref="ISecureRandomGenerator" />.
    /// </summary>
    public sealed class SecureRandomGenerator : ISecureRandomGenerator, IDisposable
    {
        private readonly RNGCryptoServiceProvider rngCrypt = new RNGCryptoServiceProvider();

        /// <inheritdoc />
        public byte[] GetRandomBytes(int count)
        {
            var arr = new byte[count];
            this.rngCrypt.GetBytes(arr);
            return arr;
        }

        /// <inheritdoc />
        public void Dispose() => this.rngCrypt.Dispose();
    }
}
