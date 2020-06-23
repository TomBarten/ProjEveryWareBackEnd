// <copyright file="MathController.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.API.V1.Models;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Represents a controller capable of multiplying two numbers together.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MathController : ControllerBase
    {
        private readonly IMathManager mathManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathController"/> class.
        /// </summary>
        /// <param name="mathManager">The math manager to use.</param>
        public MathController(
            IMathManager mathManager)
        {
            this.mathManager = mathManager ?? throw new ArgumentNullException(nameof(mathManager));
        }

        /// <summary>
        /// Multiplies two numbers together.
        /// </summary>
        /// <param name="factorA">The first factor.</param>
        /// <param name="factorB">The second factor.</param>
        /// <returns>The multiplication result.</returns>
        /// <response code="200">When the operation completed sucessfully.</response>
        /// <response code="400">When the request could not be parsed.</response>
        [HttpGet("multiply")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<MathResult> Multiply(
            [FromQuery] int factorA,
            [FromQuery] int factorB)
            => new MathResult()
            {
                Result = this.mathManager.Multiply(factorA, factorB),
            };

        /// <summary>
        /// Divides two numbers.
        /// </summary>
        /// <param name="dividend">The dividend.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>The division result.</returns>
        /// <response code="200">When the operation completed sucessfully.</response>
        /// <response code="400">When the request could not be parsed or the divisor is zero.</response>
        [HttpGet("divide")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<MathResult> Divide(
            [FromQuery] int dividend,
            [FromQuery] int divisor)
        {
            if (divisor == 0)
            {
                return this.BadRequest("Cannot divide by zero.");
            }

            return new MathResult()
            {
                Result = this.mathManager.Divide(dividend, divisor),
            };
        }

        /// <summary>
        /// Gets a set of primes.
        /// </summary>
        /// <param name="count">The amount of requested primes.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <exception cref="OperationCanceledException">When a request to cancel is fulfilled.</exception>
        /// <returns>The requested amount of prime numbers.</returns>
        /// <response code="200">When the operation completed sucessfully.</response>
        /// <response code="400">
        /// When the request could not be parsed or the amount of requested primes does not lie between 1 and 3 (inclusive).
        /// </response>
        [HttpGet("primes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<long>>> Primes(
            [FromQuery] int count,
            CancellationToken cancellationToken)
        {
            if (count < 1 || count > 3)
            {
                return this.BadRequest("Requested amount of primes must lie between 1 and 3 (inclusive).");
            }

            return await this.mathManager.GetPrimesAsync(count, cancellationToken).ConfigureAwait(false);
        }
    }
}
