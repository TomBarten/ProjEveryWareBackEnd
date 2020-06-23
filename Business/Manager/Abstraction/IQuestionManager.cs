// <copyright file="IQuestionManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Abstraction
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Manager.Implementation;
    using Fvect.Backend.Data.Database.Model;

    /// <summary>
    /// Represents an object that can do question-related operations.
    /// </summary>
    public interface IQuestionManager : IResourceManager<Question>
    {
        /// <summary>
        /// Gets a set of questions by id.
        /// </summary>
        /// <param name="questionIds">A collection of identifiers of multiple questions.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> representing the result of the asynchronous operation.</returns>
        public IAsyncEnumerable<Question?> FindMultipleByIds(IEnumerable<Guid> questionIds, CancellationToken cancellationToken);
    }
}
