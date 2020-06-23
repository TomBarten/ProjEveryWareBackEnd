// <copyright file="IUserProfileManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Abstraction
{
    using Fvect.Backend.Data.Database.Model;

    /// <summary>
    /// Represents an object that can do profile-related operations.
    /// </summary>
    public interface IUserProfileManager : IResourceManager<UserProfile>
    {
    }
}
