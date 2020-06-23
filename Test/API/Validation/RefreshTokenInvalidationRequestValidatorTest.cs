// <copyright file="RefreshTokenInvalidationRequestValidatorTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.Validation
{
    using System;
    using FluentValidation.TestHelper;
    using Fvect.Backend.API.V1.Validators;
    using Fvect.Backend.Test.Extensions;
    using Xunit;

    /// <summary>
    /// Represents a test for the <see cref="RefreshTokenInvalidationRequestValidator"/>.
    /// </summary>
    public class RefreshTokenInvalidationRequestValidatorTest
    {
        private readonly RefreshTokenInvalidationRequestValidator validator = new RefreshTokenInvalidationRequestValidator();

        /// <summary>
        /// Tests that an empty token produces a validation error.
        /// </summary>
        [Fact]
        public void EmptyRefreshTokenInvalidationTokenProducesError() =>
            this.validator.ShouldHaveValidationErrorFor(
                r => r.Token, string.Empty)
            .AsVoid();
    }
}
