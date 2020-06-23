// <copyright file="MathManagerTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Business.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using FsCheck;
    using FsCheck.Xunit;
    using Fvect.Backend.Business.Manager.Implementation;
    using Fvect.Backend.Data.Database;
    using Fvect.Backend.Data.Database.Model;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Represents a test for the <seealso cref="MathManager"/>.
    /// </summary>
    public class MathManagerTest
    {
        private readonly ITestOutputHelper outputHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathManagerTest"/> class.
        /// </summary>
        /// <param name="outputHelper">The test output helper.</param>
        public MathManagerTest(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
        }

        /// <summary>
        /// Tests the retrieval of prime numbers from the database.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task GetPrimesReturnsPrimesFromDatabaseAsync()
        {
            using var database = new TestDatabase(this.outputHelper);

            var expectedOutput = Enumerable.Range(0, 3).Select(x => (long)x).ToImmutableArray();

            using (var context = await database.CreateContextAsync().ConfigureAwait(true))
            {
                context.PrimeNumbers.AddRange(
                    expectedOutput
                        .Select(x => new PrimeNumber() { Value = x, }));

                await context.SaveChangesAsync().ConfigureAwait(true);
            }

            using (var context = await database.CreateContextAsync().ConfigureAwait(true))
            {
                var manager = this.CreateMathManager(context);
                var primes = await manager.GetPrimesAsync(3).ConfigureAwait(true);

                Assert.Equal(expectedOutput as IEnumerable<long>, primes as IEnumerable<long>);
            }
        }

        /// <summary>
        /// Tests that multiplying a number with factor 2 is equal to adding
        /// that number to itself.
        /// </summary>
        /// <param name="x">The test number.</param>
        /// <returns>The test property.</returns>
        [Property]
        public Property MultiplyXByTwoEqualsXPlusX(int x) =>
            this.CreatePropertyTestForMathManager(
                manager => (manager.Multiply(x, 2) == x + x).ToProperty());

        /// <summary>
        /// Tests that dividing integer x by 2 and then multiplying it by 2
        /// results in x as long as x is even.
        /// </summary>
        /// <param name="x">Test value x.</param>
        /// <returns>The test property.</returns>
        [Property]
        public Property DivideXByTwoThenMultiplyByTwoEqualsXWhenXIsEven(int x) =>
            this.CreatePropertyTestForMathManager(
                manager =>
                {
                    Func<bool> property = () => (manager.Divide(x, 2) * 2) == x;
                    return property
                        .When(x % 2 == 0);
                });

        /// <summary>
        /// Tests that attempting to divide integer x by zero
        /// causes an <see cref="ArgumentException"/> to be thrown.
        /// </summary>
        /// <param name="x">Test value x.</param>
        /// <returns>The test property.</returns>
        [Property]
        public Property AttemptToDivideXByZeroThrowsArgumentException(int x) =>
            this.CreatePropertyTestForMathManager(
                manager =>
                {
                    return Prop.Throws<ArgumentException, int>(new Lazy<int>(() => manager.Divide(x, 0)));
                });

        private Property CreatePropertyTestForMathManager(Func<MathManager, Property> propertyCreator)
        {
            using var database = new TestDatabase(this.outputHelper);
            using var context = database.CreateContext();
            return propertyCreator(this.CreateMathManager(context));
        }

        private MathManager CreateMathManager(FvectContext dbContext)
            => new MathManager(
                    dbContext,
                    LoggingTestHelper.CreateLogger<MathManager>(this.outputHelper));
    }
}
