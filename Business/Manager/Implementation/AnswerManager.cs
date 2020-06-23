// <copyright file="AnswerManager.cs" company="Fvect">
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
    public class AnswerManager : ResourceManagerBase<Answer, AnswerManager>, IAnswerManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public AnswerManager(
            FvectContext dbContext,
            ILogger<AnswerManager> logger)
            : base(dbContext, logger)
        {
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Answer?> FindMultipleByIds(IEnumerable<Guid> answerIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var answers = await this.DbSet
                .Where(answer => answerIds.Any(id => answer.Id == id))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            if (answers == null || !answers.Any())
            {
                yield break;
            }

            foreach (var answer in answers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return answer;
            }
        }
    }
}
