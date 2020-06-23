// <copyright file="AuthenticationRequestValidator.cs" company="Fvect">
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
    /// Represents a validator for the <see cref="AuthenticationRequest"/> type.
    /// </summary>
    public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationRequestValidator"/> class.
        /// </summary>
        public AuthenticationRequestValidator()
        {
            this.RuleFor(req => req.ClientApplicationId).NotEmpty();
            this.RuleFor(req => req.UserName).NotEmpty();
            this.RuleFor(req => req.Password).IsPassword();
        }
    }
}
