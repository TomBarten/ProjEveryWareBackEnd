// <copyright file="FvectContext.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database
{
    using System;
    using System.Linq;
    using Fvect.Backend.Data.Database.Model;
    using Fvect.Backend.Data.Database.SQLiteCompatibility;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

    /// <summary>
    /// Represents a context for the Fvect backend database.
    /// </summary>
    public class FvectContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        private static readonly string SQLiteDatabaseProviderName = "Microsoft.EntityFrameworkCore.Sqlite";
        private static readonly string SQLiteTimestampDefaultValue = "CURRENT_TIMESTAMP";

        /// <summary>
        /// Initializes a new instance of the <see cref="FvectContext"/> class.
        /// </summary>
        /// <param name="options">The options for this <see cref="FvectContext"/>.</param>
        public FvectContext(DbContextOptions<FvectContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the <seealso cref="DbSet{TEntity}"/> containing the prime numbers
        /// known to the application.
        /// </summary>
        public DbSet<PrimeNumber> PrimeNumbers { get; set; } = null!;

        /// <summary>
        /// Gets or sets the <seealso cref="DbSet{TEntity}"/> containing the questions
        /// known to the application.
        /// </summary>
        public DbSet<Question> Questions { get; set; } = null!;

        /// <summary>
        /// Gets or sets the <seealso cref="DbSet{TEntity}"/> containing the answers
        /// known to the application.
        /// </summary>
        public DbSet<Answer> Answers { get; set; } = null!;

        /// <summary>
        /// Gets or sets the <seealso cref="DbSet{TEntity}"/> containing the levels
        /// known to the application.
        /// </summary>
        public DbSet<Level> Levels { get; set; } = null!;

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> containing the
        /// cached map images.
        /// </summary>
        public DbSet<MapImage> MapImages { get; set; } = null!;

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> containing the
        /// user authentication events.
        /// </summary>
        public DbSet<UserAuthenticationEvent> UserAuthenticationEvents { get; set; } = null!;

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> containing the
        /// user refresh tokens.
        /// </summary>
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; } = null!;

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> containining the
        /// applications.
        /// </summary>
        public DbSet<ClientApplication> ClientApplications { get; set; } = null!;

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> containing the user profiles.
        /// </summary>
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);

            if (this.Database.ProviderName == SQLiteDatabaseProviderName)
            {
                AddSQLiteCompatibilityFeatures(modelBuilder);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

        private static void AddSQLiteCompatibilityFeatures(ModelBuilder modelBuilder)
        {
            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations.
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
            // This only supports millisecond precision, but should be sufficient for most use cases.
            // Credits to https://blog.dangl.me/archive/handling-datetimeoffset-in-sqlite-with-entity-framework-core/.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType
                    .ClrType
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?));

                var dateTimeOffsetConverter = new DateTimeOffsetToBinaryConverter();

                foreach (var property in properties)
                {
                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(dateTimeOffsetConverter);
                }
            }

            // The SQLite database provider does not properly implement concurrency tokens. Therefore, a custom
            // SQLite value converter and comparer must be used for concurrency tokens.
            // More info: https://entityframeworkcore.com/knowledge-base/52684458/updating-entity-in-ef-core-application-with-sqlite-gives-dbupdateconcurrencyexception.
            var concurrencyTokens = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(p =>
                    p.ClrType == typeof(byte[])
                    && p.ValueGenerated == ValueGenerated.OnAddOrUpdate
                    && p.IsConcurrencyToken);

            var concurrencyTokenConverter = new SQLiteConcurrencyTokenConverter();
            var concurrencyTokenComparer = new SQLiteConcurrencyTokenComparer();
            foreach (var property in concurrencyTokens)
            {
                property.SetValueConverter(concurrencyTokenConverter);
                property.SetValueComparer(concurrencyTokenComparer);
                property.SetDefaultValueSql(SQLiteTimestampDefaultValue);
            }
        }
    }
}
