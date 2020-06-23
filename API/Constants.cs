// <copyright file="Constants.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API
{
    /// <summary>
    /// Contains constants for the API.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The time interval that is sent in the caching header
        /// for longlived content that is not subject to change
        /// on a regular basis.
        /// </summary>
        internal const int CacheTimeLonglivedContent = 1209600; // 14 days

        /// <summary>
        /// The MIME type for a JPEG image.
        /// </summary>
        internal const string ImageJpegMIMEType = "image/jpeg";

        /// <summary>
        /// The MIME type for JSON.
        /// </summary>
        internal const string JSONMIMEType = "application/json";
    }
}
