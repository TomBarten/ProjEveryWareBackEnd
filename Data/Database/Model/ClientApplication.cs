// <copyright file="ClientApplication.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fvect.Backend.Data.Database.Base;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Represents a client application.
    /// </summary>
    [SuppressMessage(
        "Usage",
        "CA2227:Collection properties should be read only",
        Justification = "Required by EF Core.")]
    public class ClientApplication : ResourceEntityBase<ClientApplication>
    {
        /// <summary>
        /// Gets or sets the name of the client application.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the authentication events related to this application.
        /// </summary>
        public List<UserAuthenticationEvent> AuthenticationEvents { get; set; } = null!;

        /// <summary>
        /// Gets or sets the refresh tokens that were created for users of this application.
        /// </summary>
        public List<UserRefreshToken> RefreshTokens { get; set; } = null!;

        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<ClientApplication> builder)
        {
            base.Configure(builder);

            (builder ?? throw new ArgumentNullException(nameof(builder)))
                .HasIndex(e => e.Name)
                .IsUnique();
        }
    }
}
