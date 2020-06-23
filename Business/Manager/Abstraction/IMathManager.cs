// <copyright file="IMathManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Abstraction
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an object that can do math-related operations.
    /// </summary>
    public interface IMathManager
    {
        /// <summary>
        /// Multiplies two factors.
        /// </summary>
        /// <param name="factorA">The first factor.</param>
        /// <param name="factorB">The second factor.</param>
        /// <returns>The multiplication result.</returns>
        public int Multiply(int factorA, int factorB);

        /// <summary>
        /// Divides a dividend by a divisor.
        /// </summary>
        /// <param name="dividend">The dividend.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>The division result.</returns>
        /// <exception cref="ArgumentException">When <paramref name="divisor"/> is zero.</exception>
        public int Divide(int dividend, int divisor);

        /// <summary>
        /// Gets a set of prime numbers.
        /// </summary>
        /// <param name="count">The amount of primes that should be returned. At most three numbers can be requested.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="count"/> is smaller then one or greater then three.</exception>
        /// <exception cref="OperationCanceledException">When a cancellation request is fulfilled.</exception>
        /// <returns>A set of primes.</returns>
        public Task<ImmutableArray<long>> GetPrimesAsync(int count, CancellationToken cancellationToken = default);
    }
}
