// <copyright file="ITeacherManager.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Manager.Abstraction
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fvect.Backend.Data.Database.Model;

    /// <summary>
    /// Represents an object that can do teacher-related operations.
    /// </summary>
    public interface ITeacherManager : IResourceManager<Teacher>
    {
    }
}
