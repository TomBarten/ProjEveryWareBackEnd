// <copyright file="CreateUserRequestValidator.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Validators
{
    using FluentValidation;
    using Fvect.Backend.API.V1.Models;
    using Fvect.Backend.API.V1.Validators.Rules;

    /// <summary>
    /// Represents a validator for the <see cref="CreateUserRequest"/> model.
    /// </summary>
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUserRequestValidator"/> class.
        /// </summary>
        public CreateUserRequestValidator()
        {
            this.RuleFor(r => r.UserName).NotEmpty();
            this.RuleFor(r => r.Password).IsPassword();
        }
    }
}
