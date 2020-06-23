// <copyright file="JwtCryptoProvider.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.AuthNR.Implementation
{
    using System;
    using System.Text;
    using Fvect.Backend.API.AuthNR.Abstraction;
    using Fvect.Backend.Common.Options;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Provides cryptography requirements for JWTs.
    /// </summary>
    public class JwtCryptoProvider : IJwtCryptoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtCryptoProvider"/> class.
        /// </summary>
        /// <param name="optionsProvider">The options provider.</param>
        public JwtCryptoProvider(IOptionsMonitor<BackendOptions> optionsProvider)
        {
            this.SecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    (optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider)))
                        .CurrentValue.AuthNR.JWTSigningKey));
        }

        /// <inheritdoc />
        public string Algorithm => SecurityAlgorithms.HmacSha512Signature;

        /// <inheritdoc />
        public SecurityKey SecurityKey { get; }
    }
}
