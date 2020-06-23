// <copyright file="ResourceManagerTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Business.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Manager.Implementation;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.EntityFrameworkCore;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Represents a test for the <seealso cref="ResourceManagerBase{TEntity, TImpl}"/>.
    /// </summary>
    public class ResourceManagerTest
    {
        private readonly ITestOutputHelper outputHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManagerTest"/> class.
        /// </summary>
        /// <param name="outputHelper">The test output helper.</param>
        public ResourceManagerTest(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
        }

        /// <summary>
        /// Tests the retrieval of entities from the database.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task GetEntitiesReturnsEntitiesFromDatabaseAsync()
        {
            using var database = new TestDatabase(this.outputHelper);

            var dummyLevels = CreateEntityCollection<Level>(this.AddLevelsWithQuestions).ToList();

            var expectedLevelNumbers = dummyLevels.Where(level => level.Number >= 1)
                .Select(level => level.Number);

            var expectedQuestionCount = dummyLevels.Where(level => level.Number >= 1)
                .Sum(question => question.Questions.Count());

            using (var context = await database.CreateContextAsync().ConfigureAwait(true))
            {
                context.Levels.AddRange(dummyLevels);

                await context.SaveChangesAsync().ConfigureAwait(true);
            }

            using (var context = await database.CreateContextAsync().ConfigureAwait(true))
            {
                var manager = new LevelManager(context, LoggingTestHelper.CreateLogger<LevelManager>(this.outputHelper));

                var levels = new List<Level>();

                // All levels except level 0 including Questions property, will expect to take 10 will debug log that this is 9
                foreach (var level in (await manager.GetPagedEntitiesPage(10, 0, default, new Func<IQueryable<Level>, IQueryable<Level>>(query => query.Where(level => level.Number >= 1).Include("Questions"))).ConfigureAwait(false)).Data)
                {
                    if (level == null)
                    {
                        continue;
                    }

                    levels.Add(level);
                }

                Assert.Equal(expectedLevelNumbers, levels.Select(level => level.Number).OrderBy(x => x));
                Assert.Equal(expectedQuestionCount, levels.Sum(question => question.Questions.Count()));
            }
        }

        private static IEnumerable<T> CreateEntityCollection<T>(Action<ICollection<T>> addEntityToCollection)
        {
            ICollection<T> entities = new List<T>();

            addEntityToCollection(entities);

            return entities;
        }

        private void AddLevelsWithQuestions(ICollection<Level> levels)
        {
            // level 0 - 9
            for (var i = 0; i < 10; i++)
            {
                var questions = new List<Question>();

                // For include properties call
                for (var j = 0; j < 10; j++)
                {
                    questions.Add(new Question());
                }

                levels.Add(new Level(i, questions) { RowVersion = new byte[] { 0 } });
            }
        }
    }
}
