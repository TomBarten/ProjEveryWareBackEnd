// <copyright file="IGeoMapImageCacheCleaningTask.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Task.Abstraction
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Specifies a task that cleans up the cache of map images.
    /// </summary>
    public interface IGeoMapImageCacheCleaningTask
    {
        /// <summary>
        /// Asynchronously runs the task.
        /// </summary>
        /// <param name="ct">A cancellation token that can be used to request cancellation of the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution.</returns>
        Task RunAsync(CancellationToken ct);
    }
}
