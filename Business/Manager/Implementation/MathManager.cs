// <copyright file="MathManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Implementation
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Data.Database;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents a reference ipmlementation for an <see cref="IMathManager" />.
    /// </summary>
    public class MathManager : IMathManager
    {
        private readonly FvectContext dbContext;
        private readonly ILogger<MathManager> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public MathManager(
            FvectContext dbContext,
            ILogger<MathManager> logger)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public int Multiply(int factorA, int factorB)
            => factorA * factorB;

        /// <inheritdoc />
        public int Divide(int dividend, int divisor)
            => divisor != 0
                ? dividend / divisor
                : throw new ArgumentException("May not be equal to zero", nameof(divisor));

        /// <inheritdoc />
        public async Task<ImmutableArray<long>> GetPrimesAsync(int count, CancellationToken cancellationToken = default)
        {
            if (count < 1 || count > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Must be between 1 and 3 (inclusive).");
            }

            cancellationToken.ThrowIfCancellationRequested();

            this.logger.LogInformation("Going to retrieve {primeRequestCount} prime numbers from the database.", count);

            var numbers = await this.dbContext
                .PrimeNumbers
                .Take(count)
                .Select(x => x.Value)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            this.logger.LogInformation("Retrieved {primeReceivedCount} prime numbers from the database.", numbers.Length);

            if (numbers.Length < count)
            {
                this.logger.LogWarning(
                    "Retrieved {primeReceivedCount} prime numbers from the database, this is less then the requested amount of {primeRequestCount}.",
                    numbers.Length,
                    count);
            }

            return numbers.ToImmutableArray();
        }
    }
}
