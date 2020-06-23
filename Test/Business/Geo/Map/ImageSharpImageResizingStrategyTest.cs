// <copyright file="ImageSharpImageResizingStrategyTest.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Test.Business.Geo.Map
{
    using System.IO;
    using FluentAssertions;
    using Fvect.Backend.Business.Geo.Map.Implementation;
    using SixLabors.ImageSharp;
    using Xunit;

    /// <summary>
    /// Contains tests for the <see cref="ImageSharpImageResizingStrategy"/> class.
    /// </summary>
    public class ImageSharpImageResizingStrategyTest
    {
        private static readonly int DesiredHeight = 400;
        private static readonly int DesiredWidth = 300;

        /// <summary>
        /// Tests the <see cref="ImageSharpImageResizingStrategy.ResizeJPEGImage"/> function
        /// by resizing an image.
        /// </summary>
        [Fact]
        public void ImageResizingTest()
        {
            using var testImgStream = new FileStream(
                Path.Combine("Assets", "HEREMap.jpg"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var outputStream = new MemoryStream();

            var strategy = new ImageSharpImageResizingStrategy();
            strategy.ResizeJPEGImage(testImgStream, outputStream, DesiredHeight, DesiredWidth);

            outputStream.Position = 0;

            using var img = Image.Load(outputStream);

            img.Height.Should().Be(DesiredHeight);
            img.Width.Should().Be(DesiredWidth);
        }
    }
}
