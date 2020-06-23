// <copyright file="ObjectExtensions.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Extensions
{
    /// <summary>
    /// Contains extension methods for the <see cref="object"/> type.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Ignores a value.
        /// </summary>
        /// <param name="o">The value.</param>
        public static void AsVoid(this object o)
        {
            _ = o;
        }
    }
}
