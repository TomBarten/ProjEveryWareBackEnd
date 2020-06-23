// <copyright file="UserProfile.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.Model
{
    using System;
    using Fvect.Backend.Data.Database.Base;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Represents a user profile that contains the saved game state.
    /// </summary>
    public class UserProfile : ResourceEntityBase<UserProfile>
    {
        /// <summary>
        /// Gets or sets the identifier of the user related to this
        /// entity.
        /// </summary>
        public Guid AppUserId { get; set; }

        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        public string Value { get; set; } = null!;

        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            base.Configure(builder);

            builder
                .HasOne<AppUser>()
                .WithOne()
                .HasForeignKey<UserProfile>(x => x.AppUserId);
        }
    }
}
