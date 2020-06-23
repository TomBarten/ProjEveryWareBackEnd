// <copyright file="TestDatabase.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test
{
    using System;
    using System.Threading.Tasks;
    using Fvect.Backend.Data.Database;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    /// <summary>
    /// Represents an in-memory SQLite database used for testing purposes.
    /// The associated database is deleted from memory when the associated
    /// <see cref="TestDatabase"/> object is disposed.
    /// This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// Migrations are not used, instead the current model state is applied to the
    /// SQLite database because not all migration features are supported by SQLite.
    /// For more information about SQLite limitations, please refer to
    /// https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations.
    /// </remarks>
    /// <remarks>
    /// Because some testing frameworks might not (easisly) support asynchronous operations,
    /// public members of this class are implemented both synchronously and asynchronously.
    /// It is preferred to use the asynchronous operations wherever possible.
    /// </remarks>
    public sealed class TestDatabase : IDisposable, IAsyncDisposable
    {
        private static readonly string InMemoryConnectionString = "DataSource=:memory:";
        private readonly ILoggerFactory loggerFactory;
        private SqliteConnection? connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDatabase"/> class.
        /// </summary>
        /// <param name="outputHelper">The test output helper to use.</param>
        public TestDatabase(ITestOutputHelper outputHelper)
        {
            this.loggerFactory = LoggingTestHelper.CreateLoggerFactory(outputHelper ??
                throw new ArgumentNullException(nameof(outputHelper)));
        }

        /// <summary>
        /// Synchronously checks if the database needs to be created,
        /// asynchronously creates the database if required
        /// and synchronously returns a new <see cref="FvectContext"/>.
        /// </summary>
        /// <returns>The created <see cref="FvectContext"/>.</returns>
        /// <remarks>
        /// This method only executes asynchronously the first time it is called
        /// per instance of <see cref="TestDatabase"/>.
        /// </remarks>
        public async Task<FvectContext> CreateContextAsync()
        {
            await this.SetupIfNecessaryAsync().ConfigureAwait(false);
            return this.CreateContextPrivate();
        }

        /// <summary>
        /// Synchronously check if the database needs to be created,
        /// creates the database if required
        /// and returns a new <see cref="FvectContext"/>.
        /// </summary>
        /// <returns>The created <see cref="FvectContext"/>.</returns>
        public FvectContext CreateContext()
        {
            this.SetupIfNecessary();
            return this.CreateContextPrivate();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Disposing this instance destroys the related in-memory database.
        /// </remarks>
        public ValueTask DisposeAsync()
        {
            this.loggerFactory?.Dispose();
            return this.connection?.DisposeAsync() ?? new ValueTask(Task.CompletedTask);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Disposing this instance destroys the related in-memory database.
        /// </remarks>
        public void Dispose()
        {
            this.loggerFactory?.Dispose();
            this.connection?.Dispose();
        }

        private async Task SetupIfNecessaryAsync()
        {
            if (this.connection is null)
            {
                this.PrepareConnection();
                await this.connection!.OpenAsync().ConfigureAwait(false);
                using var ctx = this.CreateContextPrivate();
                await ctx.Database.EnsureCreatedAsync().ConfigureAwait(false);
            }
        }

        private void SetupIfNecessary()
        {
            if (this.connection is null)
            {
                this.PrepareConnection();
                this.connection!.OpenAsync();
                using var ctx = this.CreateContextPrivate();
                ctx.Database.EnsureCreated();
            }
        }

        private void PrepareConnection() =>
            this.connection = new SqliteConnection(InMemoryConnectionString);

        private FvectContext CreateContextPrivate() =>
            new FvectContext(
                new DbContextOptionsBuilder<FvectContext>()
                    .UseSqlite(
                        this.connection ?? throw new InvalidOperationException(
                            $"Instance variable \'{this.connection}\' has not been initialized."))
                    .UseLoggerFactory(this.loggerFactory)
                        .Options);
    }
}
