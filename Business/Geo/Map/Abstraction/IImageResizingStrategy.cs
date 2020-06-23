// <copyright file="IImageResizingStrategy.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Geo.Map.Abstraction
{
    using System.IO;

    /// <summary>
    /// Specifies an algortithm that can resize a JPEG image.
    /// </summary>
    public interface IImageResizingStrategy
    {
        /// <summary>
        /// Rezises a JPEG image.
        /// </summary>
        /// <param name="imageInputStream">The image.</param>
        /// <param name="imageOutputStream">The stream to write the resized image to.</param>
        /// <param name="outputHeight">The desired output height.</param>
        /// <param name="outputWidth">The desired output width.</param>
        void ResizeJPEGImage(
            Stream imageInputStream,
            Stream imageOutputStream,
            int outputHeight,
            int outputWidth);
    }
}
