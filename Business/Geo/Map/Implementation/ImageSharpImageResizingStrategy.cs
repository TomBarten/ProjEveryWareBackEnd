// <copyright file="ImageSharpImageResizingStrategy.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Business.Geo.Map.Implementation
{
    using System.IO;
    using Fvect.Backend.Business.Geo.Map.Abstraction;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;

    /// <summary>
    /// Represents an <see cref="IImageResizingStrategy"/> that uses
    /// the ImageSharp library to resize images.
    /// </summary>
    public class ImageSharpImageResizingStrategy : IImageResizingStrategy
    {
        /// <inheritdoc />
        public void ResizeJPEGImage(
            Stream imageInputStream,
            Stream imageOutputStream,
            int outputHeight,
            int outputWidth)
        {
            using var img = Image.Load(imageInputStream);
            img.Mutate(x => x.Resize(outputWidth, outputHeight));
            img.SaveAsJpeg(imageOutputStream);
        }
    }
}
