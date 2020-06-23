// <copyright file="LevelManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents a reference implementation for an <see cref="ResourceManagerBase{TEntity, TImpl}" />.
    /// </summary>
    public class LevelManager : ResourceManagerBase<Level, LevelManager>, ILevelManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public LevelManager(
            FvectContext dbContext,
            ILogger<LevelManager> logger)
            : base(dbContext, logger)
        {
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Level?> FindMultipleByIds(IEnumerable<int> levelNumbers, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var levels = await this.DbSet
                .Where(level => levelNumbers.Any(number => level.Number == number))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            if (levels == null || !levels.Any())
            {
                yield break;
            }

            foreach (var level in levels)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return level;
            }
        }
    }
}
