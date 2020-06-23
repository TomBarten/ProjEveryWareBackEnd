// <copyright file="MathControllerTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.Controller.V1
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Net;
    using System.Threading.Tasks;
    using Fvect.Backend.API.V1.Controllers;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    /// <summary>
    /// Represents a test for the <see cref="MathController"/>.
    /// </summary>
    public class MathControllerTest
    {
        /// <summary>
        /// Tests that invoking <see cref="MathController.Divide(int, int)"/>
        /// with a non-zero divisor results in an expected mocked result.
        /// </summary>
        [Fact]
        public void DivideNonZeroDivisorReturnsOKResult()
        {
            var (dividend, divisor, expectedResult) = (10, 5, 2);
            var (mock, controller) = CreateControllerAndManagerMock();

            mock.Setup(x => x.Divide(dividend, divisor))
                .Returns(expectedResult)
                .Verifiable();

            var result = controller.Divide(dividend, divisor);

            Assert.NotNull(result.Value);
            Assert.Equal(expectedResult, result.Value.Result);

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests that invoking <seealso cref="MathController.Divide(int, int)"/>
        /// with a zero divisor results in a 400 Bad Request result.
        /// </summary>
        [Fact]
        public void DivideZeroDivisorReturns400BadRequest()
        {
            var (dividend, divisor) = (10, 0);
            var (mock, controller) = CreateControllerAndManagerMock();

            var result = controller.Divide(dividend, divisor);

            var httpResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(httpResult.StatusCode, (int)HttpStatusCode.BadRequest);
            var message = Assert.IsType<string>(httpResult.Value);
            Assert.Equal("Cannot divide by zero.", message);

            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests that invoking <see cref="MathController.Multiply(int, int)"/>
        /// with two numbers results in an expected mocked result.
        /// </summary>
        [Fact]
        public void MultiplyXAndYReturnsOKResult()
        {
            var (factorA, factorB, expectedResult) = (2, 3, 6);
            var (mock, controller) = CreateControllerAndManagerMock();

            mock.Setup(x => x.Multiply(factorA, factorB))
                .Returns(expectedResult)
                .Verifiable();

            var result = controller.Multiply(factorA, factorB);

            Assert.NotNull(result.Value);
            Assert.Equal(expectedResult, result.Value.Result);

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests that requesting an amount of primes outside the allowed range
        /// results in a 400 Bad Request result.
        /// </summary>
        /// <param name="amount">The invalid amount of primes requested.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task PrimesRequestAmountOutOfRangeReturns400BadRequest(int amount)
        {
            var (mock, controller) = CreateControllerAndManagerMock();

            var result = await controller.Primes(amount, default).ConfigureAwait(true);

            var httpResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(httpResult.StatusCode, (int)HttpStatusCode.BadRequest);
            var message = Assert.IsType<string>(httpResult.Value);
            Assert.Equal("Requested amount of primes must lie between 1 and 3 (inclusive).", message);

            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests that requesting primes for an amount within the allowed range
        /// results in an expected mocked result.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task PrimesRequestAmountWithinRangeReturnsExpectedResult()
        {
            var (requestedAmount, expectedResult) = (2, ImmutableArray.Create(1L, 2L));
            var (mock, controller) = CreateControllerAndManagerMock();

            mock.Setup(x => x.GetPrimesAsync(requestedAmount, default))
                .ReturnsAsync(expectedResult)
                .Verifiable();

            var result = await controller.Primes(requestedAmount, default).ConfigureAwait(true);

            Assert.NotNull(result.Value);
            Assert.Equal(expectedResult as IEnumerable<long>, result.Value);

            mock.Verify();
            mock.VerifyNoOtherCalls();
        }

        private static (Mock<IMathManager> mock, MathController controller) CreateControllerAndManagerMock()
        {
            var mock = new Mock<IMathManager>();
            var controller = new MathController(mock.Object);

            return (mock, controller);
        }
    }
}
