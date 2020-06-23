// <copyright file="CallerInfo.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.AuthNR.Implementation.Private
{
    /// <summary>
    /// Represents metadata of the caller.
    /// </summary>
    internal struct CallerInfo
    {
        /// <summary>
        /// Gets or sets the IP Address of the caller.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent of the caller.
        /// </summary>
        public string UserAgent { get; set; }
    }
}
