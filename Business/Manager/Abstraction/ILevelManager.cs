// <copyright file="ILevelManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Abstraction
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Manager.Implementation;
    using Fvect.Backend.Data.Database.Model;

    /// <summary>
    /// Represents an object that can do level-related operations.
    /// </summary>
    public interface ILevelManager : IResourceManager<Level>
    {
        /// <summary>
        /// Gets a set of levels by id.
        /// </summary>
        /// <param name="levelNumbers">A collection of identifiers of multiple levels.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> representing the result of the asynchronous operation.</returns>
        public IAsyncEnumerable<Level?> FindMultipleByIds(IEnumerable<int> levelNumbers, CancellationToken cancellationToken);
    }
}
