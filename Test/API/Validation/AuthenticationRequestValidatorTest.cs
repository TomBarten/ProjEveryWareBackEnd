﻿// <copyright file="AuthenticationRequestValidatorTest.cs" company="Fvect">
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
    /// Represents a test for the <see cref="AuthenticationRequestValidator"/>.
    /// </summary>
    public class AuthenticationRequestValidatorTest
    {
        private readonly AuthenticationRequestValidator validator = new AuthenticationRequestValidator();

        /// <summary>
        /// Tests that an empty client application id produces a validation error.
        /// </summary>
        [Fact]
        public void EmptyClientApplicationIdProducesError() => this.validator.ShouldHaveValidationErrorFor(
                r => r.ClientApplicationId, default(Guid))
            .AsVoid();

        /// <summary>
        /// Tests that an empty user name produces a validation error.
        /// </summary>
        [Fact]
        public void EmptyUserNameProducesError() =>
            this.validator.ShouldHaveValidationErrorFor(
                r => r.UserName, string.Empty)
            .AsVoid();

        /// <summary>
        /// Tests that a password of 7 characters causes a validation error.
        /// property.
        /// </summary>
        [Fact]
        public void PasswordHasChildValidator() =>
            this.validator.ShouldHaveValidationErrorFor(
                r => r.Password, "1234567")
            .AsVoid();
    }
}
