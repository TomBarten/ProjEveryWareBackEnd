// <copyright file="AppRole.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.Model
{
    using System;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Represents a Role of the application.
    /// </summary>
    public class AppRole : IdentityRole<Guid>
    {
    }
}
