// <copyright file="JwtCryptoProviderExtensionsTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.API.AuthNR.Extensions
{
    using FluentAssertions;
    using Fvect.Backend.API.AuthNR.Abstraction;
    using Fvect.Backend.API.AuthNR.Extensions;
    using Microsoft.IdentityModel.Tokens;
    using Moq;
    using Xunit;

    /// <summary>
    /// Contains tests for the extension methods defined in the static
    /// <see cref="JwtCryptoProviderExtensions"/> class.
    /// </summary>
    public class JwtCryptoProviderExtensionsTest
    {
        /// <summary>
        /// Tests the <see cref="JwtCryptoProviderExtensions.CreateSigningCredentials"/> method.
        /// </summary>
        [Fact]
        public void CreateSigningCredentialsTest()
        {
            var weakKey = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };
            var symmetricalKey = new SymmetricSecurityKey(weakKey);
            var algo = SecurityAlgorithms.HmacSha512Signature;

            var jwtCryptoMock = new Mock<IJwtCryptoProvider>();

            jwtCryptoMock.SetupGet(m => m.Algorithm)
                .Returns(algo)
                .Verifiable();

            jwtCryptoMock.SetupGet(m => m.SecurityKey)
                .Returns(symmetricalKey)
                .Verifiable();

            var signingCredentials = jwtCryptoMock.Object.CreateSigningCredentials();

            signingCredentials.Algorithm
                .Should()
                .Be(
                    algo,
                    because: "this was included in the provider");

            signingCredentials.Key
                .Should()
                .BeSameAs(
                    symmetricalKey,
                    because: "this was included in the provider");

            jwtCryptoMock.Verify();
            jwtCryptoMock.VerifyNoOtherCalls();
        }
    }
}
