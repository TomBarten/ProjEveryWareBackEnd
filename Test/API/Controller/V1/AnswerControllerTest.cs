// <copyright file="AnswerControllerTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.Controller.V1
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.API.V1.Controllers;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Common.Models;
    using Fvect.Backend.Data.Database.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Moq;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Represents a test for the <see cref="AnswerController"/>.
    /// </summary>
    public class AnswerControllerTest
    {
        /// <summary>
        /// Tests if the AnswerController returns the expected paged data with answers.
        /// </summary>
        /// <param name="amount">The amount of answers requested.</param>
        /// <param name="pageSize">The pageSize of the paged data requested.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(10, 10)]
        [InlineData(10, 20)]
        [InlineData(8, 4)]
        public async Task GetAnswersPagedTest(int amount, int pageSize)
        {
            var pageIndex = 0;
            var (mock, controller) = CreateControllerAndManagerMock();
            var expectedAnswers = GetMockAnswers(amount);

            mock.Setup(x => x.GetPagedEntitiesPage(pageSize, pageIndex, default, default))
                .Returns(Task.FromResult(new PagedData<Answer>(0, (int)Math.Ceiling((decimal)amount / pageSize), pageSize, expectedAnswers.ToImmutableArray())));

            var result = await controller.GetEntities(default, pageSize, pageIndex).ConfigureAwait(false);

            Assert.NotNull(result.Value);
            Assert.Equal(expectedAnswers, result.Value.Data);
            Assert.Equal(0, result.Value.PageNumber);
            Assert.Equal(pageSize, result.Value.DataPerPage);
            Assert.Equal(Math.Ceiling((decimal)amount / pageSize), result.Value.TotalPageCount);
        }

        /// <summary>
        /// Tests if the AnswerController returns a bad request with a page index outside the allowed range.
        /// </summary>
        /// <param name="pageIndex">The pageIndex of the paged data requested.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Theory]
        [InlineData(-1)]
        [InlineData(-5)]
        [InlineData(-10)]
        public async Task GetAnswersPageIndexTest(int pageIndex)
        {
            var pageSize = 10;
            var (mock, controller) = CreateControllerAndManagerMock();

            var result = await controller.GetEntities(default, pageSize, pageIndex).ConfigureAwait(false);

            var httpResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.BadRequest, httpResult.StatusCode);
            var message = Assert.IsType<string>(httpResult.Value);
            Assert.Equal("'pageIndex' may not be less then 0.", message);

            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests if the AnswerController returns a bad request with a page size outside the allowed range.
        /// </summary>
        /// <param name="pageSize">The pageSize of the paged data requested.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task GetAnswersPageSizeTest(int pageSize)
        {
            var pageIndex = 0;
            var (mock, controller) = CreateControllerAndManagerMock();

            var result = await controller.GetEntities(default, pageSize, pageIndex).ConfigureAwait(false);

            var httpResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.BadRequest, httpResult.StatusCode);
            var message = Assert.IsType<string>(httpResult.Value);
            Assert.Equal("'pageSize' may not be less then 1.", message);

            mock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Tests getting an answer by id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetAnswerByIdTest()
        {
            var (mock, controller) = CreateControllerAndManagerMock();
            var answer = GetMockAnswers(1).First();

            mock.Setup(x => x.FindById(answer.Id, default))
                .Returns(new ValueTask<Answer?>(Task.FromResult<Answer?>(answer)));

            var result = await controller.GetEntity(answer.Id, default).ConfigureAwait(false);

            Assert.Equal(answer, result.Value);
        }

        /// <summary>
        /// Tests getting an answer by id that doesn't exist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetAnswerByIdNotFoundTest()
        {
            var (mock, controller) = CreateControllerAndManagerMock();

            mock.Setup(x => x.FindById(It.IsAny<Guid>(), default))
                .Returns(new ValueTask<Answer?>(Task.FromResult<Answer?>(null)));

            var result = await controller.GetEntity(default, default).ConfigureAwait(false);

            var httpResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.NotFound, httpResult.StatusCode);
        }

        /// <summary>
        /// Tests updating an answer by id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task UpdateAnswerTest()
        {
            var (mock, controller) = CreateControllerAndManagerMock();
            var answer = GetMockAnswers(1).First();

            mock.Setup(x => x.Update(answer, default))
                .Returns(Task.FromResult(true))
                .Verifiable();

            await controller.PutEntity(answer.Id, answer, default).ConfigureAwait(false);

            mock.Verify();
        }

        /// <summary>
        /// Tests adding an answer.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task AddAnswerTest()
        {
            var (mock, controller) = CreateControllerAndManagerMock();
            var answer = new Answer
            {
                Value = "Test Answer",
            };

            mock.Setup(x => x.Add(answer, default))
                .Returns(Task.FromResult(default(Guid)))
                .Verifiable();

            await controller.PostEntity(answer, new ApiVersion(1, 0), default).ConfigureAwait(false);

            mock.Verify();
        }

        /// <summary>
        /// Tests deleting an answer by id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task DeleteAnswerTest()
        {
            var (mock, controller) = CreateControllerAndManagerMock();
            var answer = GetMockAnswers(1).First();

            mock.Setup(x => x.Remove(answer.Id, default))
                .Returns(Task.FromResult(true))
                .Verifiable();

            await controller.DeleteEntity(answer.Id, default).ConfigureAwait(false);

            mock.Verify();
        }

        private static (Mock<IAnswerManager> mock, AnswerController controller) CreateControllerAndManagerMock()
        {
            var mock = new Mock<IAnswerManager>();
            var controller = new AnswerController(mock.Object);

            return (mock, controller);
        }

        private static IEnumerable<Answer> GetMockAnswers(int amount)
        {
            var answers = new List<Answer>();
            for (int i = 0; i < amount; i++)
            {
                answers.Add(new Answer
                {
                    Id = Guid.NewGuid(),
                    Value = $"Answer {i}",
                });
            }

            return answers;
        }
    }
}
