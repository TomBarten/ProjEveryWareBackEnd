// <copyright file="QuestionManager.cs" company="Fvect">
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
    public class QuestionManager : ResourceManagerBase<Question, QuestionManager>, IQuestionManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public QuestionManager(
            FvectContext dbContext,
            ILogger<QuestionManager> logger)
            : base(dbContext, logger)
        {
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<Question?> FindMultipleByIds(IEnumerable<Guid> questionIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var questions = await this.DbSet
                .Where(question => questionIds.Any(id => question.Id == id))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            if (questions == null || !questions.Any())
            {
                yield break;
            }

            foreach (var question in questions)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return question;
            }
        }
    }
}
