// <copyright file="GeoController.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.V1.Controllers
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Business.Manager.Abstraction;
    using Fvect.Backend.Common.Models.Geo;
    using Fvect.Backend.Common.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Represents a controller that implements routes related
    /// to geographic information functionality.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class GeoController : ControllerBase
    {
        private static readonly Regex PostalCodeRegex = new Regex(
            @"^[1-9][0-9]{3}\s?[A-Za-z]{2}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private readonly IGeoManager geoManager;
        private readonly IOptionsMonitor<BackendOptions> optionsMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoController"/> class.
        /// </summary>
        /// <param name="geoManager">The geo manager.</param>
        /// <param name="optionsMonitor">The options monitor.</param>
        public GeoController(
            IGeoManager geoManager,
            IOptionsMonitor<BackendOptions> optionsMonitor)
        {
            this.geoManager = geoManager
                ?? throw new ArgumentNullException(nameof(geoManager));

            this.optionsMonitor = optionsMonitor
                ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        /// <summary>
        /// Retrieves a map image by zip code, house number and zoom level.
        /// </summary>
        /// <param name="zipCode">
        /// A Dutch zipcode.
        /// Must comply with the following regular expression:
        /// <c>^[1-9][0-9]{3}\s?[A-Za-z]{2}$</c>.
        /// </param>
        /// <param name="houseNumber">The house number.</param>
        /// <param name="zoomLevel">The desired zoom level.</param>
        /// <param name="cancellationToken">
        /// A token that can be used to request cancellation of the operation.
        /// </param>
        /// <param name="height">The desired height of the image. Optional.</param>
        /// <param name="width">The desired width of the image. Optional.</param>
        /// <returns>The requested map image.</returns>
        /// <result code="200">When the operation completed successfully.</result>
        /// <result code="400">When the parameters are not valid.</result>
        /// <result code="404">When the map cannot be found.</result>
        /// <remarks>
        /// In case of a response with code <c>200</c>, the response will be of
        /// MIME type <c>image/jpeg</c>. For all other response codes, the response
        /// will be of MIME type <c>application/json</c>.
        /// </remarks>
        /// <remarks>
        /// When no height and/or width is/are specified, the following default values
        /// will be used:
        /// <ul>
        ///     <li><c>720</c> for the height.</li>
        ///     <li><c>1280</c> for the width.</li>
        /// </ul>.
        /// The maximum requested height and/or width is configurable per API instance
        /// but has the following default values:
        /// <ul>
        ///     <li><c>1440</c> for the height.</li>
        ///     <li><c>2560</c> for the width.</li>
        /// </ul>.
        /// </remarks>
        [HttpGet("mapImage/{zipCode}/{houseNumber}/{zoomLevel}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(Constants.ImageJpegMIMEType, Constants.JSONMIMEType)]
        [ResponseCache(
            Duration = Constants.CacheTimeLonglivedContent,
            Location = ResponseCacheLocation.Any,
            VaryByQueryKeys = new[] { "height", "width" })]
        public async Task<ActionResult> GetMapImage(
            [FromRoute] string zipCode,
            [FromRoute] int houseNumber,
            [FromRoute] ZoomLevel zoomLevel,
            CancellationToken cancellationToken,
            [FromQuery] int? height = default,
            [FromQuery] int? width = default)
        {
            if (!PostalCodeRegex.IsMatch(zipCode))
            {
                return this.BadRequest(nameof(zipCode));
            }

            if (houseNumber < 1)
            {
                return this.BadRequest(nameof(houseNumber));
            }

            if (height != null && (height > this.optionsMonitor.CurrentValue.Geo.MaxRequestHeightForMapImage || height < 1))
            {
                return this.BadRequest(nameof(height));
            }

            if (width != null && (width > this.optionsMonitor.CurrentValue.Geo.MaxRequestWidthForMapImage || width < 1))
            {
                return this.BadRequest(nameof(width));
            }

            var image = await this.geoManager.GetMapImageAsync(
                    zipCode,
                    houseNumber,
                    zoomLevel,
                    height,
                    width,
                    cancellationToken)
                .ConfigureAwait(false);

            if (image is null)
            {
                return this.NotFound();
            }

            image.Position = 0;

            return this.File(
                image,
                Constants.ImageJpegMIMEType);
        }
    }
}
