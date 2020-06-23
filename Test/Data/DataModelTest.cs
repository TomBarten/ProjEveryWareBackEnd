// <copyright file="DataModelTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Data
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Fvect.Backend.Data.Database;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata.Conventions;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Xunit;

    /// <summary>
    /// Contains tests for the data model.
    /// </summary>
    public class DataModelTest
    {
        private static readonly string UpMethodName = "Up";
        private static readonly string DownMethodName = "Down";
        private static readonly string BuildTargetModelMethodName = "BuildTargetModel";
        private static readonly string DummyConnectionString = @"Server=localhost;Database=DoesNotExist;Trusted_Connection=True;";
        private static readonly string SqlServerProviderName = "Microsoft.EntityFrameworkCore.SqlServer";

        /// <summary>
        /// Tests that the current model does not contain any changes
        /// that are not contained in the database migrators.
        /// In other words: tests that the current model state equals the
        /// state that results from all the migrations combined.
        /// </summary>
        [Fact]
        public void ModelDoesNotContainPendingChanges()
        {
            // Do not use the test database, the SQL Server model provider must be
            // used as that is the model provider that is used for scaffolding migrations.
            using var ctx = new FvectContext(
                new DbContextOptionsBuilder<FvectContext>()
                    .UseSqlServer(DummyConnectionString)
                    .Options);

            var modelDiffer = ctx.GetService<IMigrationsModelDiffer>();
            var migrationsAssembly = ctx.GetService<IMigrationsAssembly>();

            var pendingModelChanges = modelDiffer
                .GetDifferences(
                    migrationsAssembly.ModelSnapshot?.Model,
                    ctx.Model);

            pendingModelChanges
                .Should()
                .BeEmpty(
                    because:
                        "the current model state should be equal to the state that results from all the migrations combined (try scaffolding a migration)");
        }

        /// <summary>
        /// Tests that all migrations in the 'Fvect.Data' assembly
        /// can execute without throwing an exception.
        /// </summary>
        [Fact]
        public void MigrationsCanExecuteWithoutThrowingAnException()
        {
            var assembly = typeof(FvectContext).Assembly;

            var migrationTypes = assembly
                .GetTypes()
                .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(Migration)))
                .ToImmutableList();

            foreach (var migrationType in migrationTypes)
            {
                var migration = (Migration)Activator.CreateInstance(migrationType) !;

                foreach (var methodName in new[] { UpMethodName, DownMethodName })
                {
                    GetMethodInfoForProtectedMigrationMethod(methodName, migration) // Thanks MSFT for making these protected.
                        .Invoke(migration, new[] { new MigrationBuilder(SqlServerProviderName) as object });
                }

                GetMethodInfoForProtectedMigrationMethod(BuildTargetModelMethodName, migration)
                    .Invoke(migration, new[] { new ModelBuilder(new ConventionSet()) as object });
            }
        }

        private static MethodInfo GetMethodInfoForProtectedMigrationMethod(string methodName, Migration migration)
            => migration
                .GetType()
                .GetMethod(
                    methodName,
                    BindingFlags.NonPublic | BindingFlags.Instance) !;
    }
}
