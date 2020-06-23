// <copyright file="RefreshRequestValidator.cs" company="Fvect">
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
    /// Represents a validator for the <see cref="RefreshRequest"/> type.
    /// </summary>
    public class RefreshRequestValidator : AbstractValidator<RefreshRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshRequestValidator"/> class.
        /// </summary>
        public RefreshRequestValidator()
        {
            this.RuleFor(req => req.ClientApplicationId).NotEmpty();
            this.RuleFor(req => req.RefreshToken).NotEmpty();
        }
    }
}
