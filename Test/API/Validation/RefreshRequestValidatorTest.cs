// <copyright file="RefreshRequestValidatorTest.cs" company="Fvect">
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
    /// Represents a test for the <see cref="RefreshRequestValidator"/>.
    /// </summary>
    public class RefreshRequestValidatorTest
    {
        private readonly RefreshRequestValidator validator = new RefreshRequestValidator();

        /// <summary>
        /// Tests that an empty client application id produces a validation error.
        /// </summary>
        [Fact]
        public void EmptyClientApplicationIdProducesError() => this.validator.ShouldHaveValidationErrorFor(
                r => r.ClientApplicationId, default(Guid))
            .AsVoid();

        /// <summary>
        /// Tests that an empty refresh token produces a validation error.
        /// </summary>
        [Fact]
        public void EmptyRefreshTokenProducesError() =>
            this.validator.ShouldHaveValidationErrorFor(
                r => r.RefreshToken, string.Empty)
            .AsVoid();
    }
}
