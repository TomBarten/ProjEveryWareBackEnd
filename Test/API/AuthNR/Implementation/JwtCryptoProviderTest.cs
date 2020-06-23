// <copyright file="JwtCryptoProviderTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.AuthNR.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using FluentAssertions;
    using Fvect.Backend.API.AuthNR.Implementation;
    using Microsoft.IdentityModel.Tokens;
    using Xunit;

    /// <summary>
    /// Contains tests for the <see cref="JwtCryptoProvider"/> class.
    /// </summary>
    public class JwtCryptoProviderTest
    {
        /// <summary>
        /// Asserts that the <see cref="JwtCryptoProvider.Algorithm"/> property
        /// returns the value of <see cref="SecurityAlgorithms.HmacSha512Signature"/>.
        /// </summary>
        [Fact]
        public void AlgorithmEqualsHmacSha512Signature()
        {
            var options = OptionsTestHelper.CreateBackendOptionsMock();

            var provider = new JwtCryptoProvider(options.Object);

            provider.Algorithm
                .Should()
                .Be(
                    SecurityAlgorithms.HmacSha512Signature,
                    because: "this is the chosen security algorithm");
        }

        /// <summary>
        /// Tests that the correct security key from the options is
        /// returned from the <see cref="JwtCryptoProvider.SecurityKey"/> property.
        /// </summary>
        [Fact]
        public void SecurityKeyIsSymmetricKeyProvidedInOptions()
        {
            var options = OptionsTestHelper.CreateBackendOptionsMock();

            var provider = new JwtCryptoProvider(options.Object);

            provider.SecurityKey
                .Should()
                .BeOfType<SymmetricSecurityKey>(because: "we use a symettric security key");

            var symmetricalKey = (SymmetricSecurityKey)provider.SecurityKey;

            symmetricalKey.Key.Should()
                .BeEquivalentTo(
                    Encoding.UTF8.GetBytes(options.Object.CurrentValue.AuthNR.JWTSigningKey),
                    because: "this is the key configured in the options");
        }
    }
}
