// <copyright file="JwtCryptoProviderExtensions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.AuthNR.Extensions
{
    using System;
    using Fvect.Backend.API.AuthNR.Abstraction;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Contains extension methods for the <see cref="IJwtCryptoProvider"/> type.
    /// </summary>
    public static class JwtCryptoProviderExtensions
    {
        /// <summary>
        /// Creates a <see cref="SigningCredentials"/> from this <see cref="IJwtCryptoProvider"/>.
        /// </summary>
        /// <param name="jwtCryptoProvider">
        /// The target <see cref="IJwtCryptoProvider"/>.
        /// </param>
        /// <returns>
        /// The created <see cref="SigningCredentials"/>.
        /// </returns>
        public static SigningCredentials CreateSigningCredentials(
            this IJwtCryptoProvider jwtCryptoProvider) =>
            new SigningCredentials(
                (jwtCryptoProvider ?? throw new ArgumentNullException(nameof(jwtCryptoProvider))).SecurityKey,
                jwtCryptoProvider.Algorithm);
    }
}
