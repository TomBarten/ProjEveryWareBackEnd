// <copyright file="IJwtCryptoProvider.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.AuthNR.Abstraction
{
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Specifies an object that can provide JWT-crypto requirements.
    /// </summary>
    public interface IJwtCryptoProvider
    {
        /// <summary>
        /// Gets the security algorithm that is to be used when creating and/or
        /// validating JWTs.
        /// </summary>
        string Algorithm { get; }

        /// <summary>
        /// Gets the security key that is to be used when creating and/or
        /// validating JWTs.
        /// </summary>
        SecurityKey SecurityKey { get; }
    }
}
