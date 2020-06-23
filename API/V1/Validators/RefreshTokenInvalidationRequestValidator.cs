// <copyright file="RefreshTokenInvalidationRequestValidator.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Validators
{
    using FluentValidation;
    using Fvect.Backend.API.V1.Models;

    /// <summary>
    /// Represents a validator for the <see cref="RefreshTokenInvalidationRequest"/>.
    /// </summary>
    public class RefreshTokenInvalidationRequestValidator : AbstractValidator<RefreshTokenInvalidationRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenInvalidationRequestValidator"/> class.
        /// </summary>
        public RefreshTokenInvalidationRequestValidator()
        {
            this.RuleFor(r => r.Token).NotEmpty();
        }
    }
}
