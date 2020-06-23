// <copyright file="PasswordRule.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Validators.Rules
{
    using FluentValidation;

    /// <summary>
    /// Contains a validation rule for password.
    /// </summary>
    public static class PasswordRule
    {
        /// <summary>
        /// Validates the rule as a password.
        /// </summary>
        /// <typeparam name="T">The type of the model.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>The builder options.</returns>
        public static IRuleBuilderOptions<T, string> IsPassword<T>(this IRuleBuilder<T, string> builder) =>
            builder.MinimumLength(8);
    }
}
