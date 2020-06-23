// <copyright file="IGeocodeClient.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.HERE.Abstraction
{
    using System.Threading;
    using System.Threading.Tasks;
    using Fvect.Backend.Common.Exception;
    using Fvect.Backend.Data.HERE.Models;

    /// <summary>
    /// Specifies a client for the HERE Geocode API.
    /// </summary>
    public interface IGeocodeClient
    {
        /// <summary>
        /// Retrieves geocode information from the Geocode service.
        /// </summary>
        /// <param name="dutchPostalCode">The dutch postal code. The letters may be ommitted.</param>
        /// <param name="houseNumber">The house number. May be <c>null</c>.</param>
        /// <param name="ct">A token that can be used to request cancellation of the operation.</param>
        /// <returns>The Geocode information, or <c>null</c> when it could not be found.</returns>
        /// <exception cref="DataProviderException">
        /// When there was an error communicating with HERE Maps.
        /// </exception>
        Task<GeocodeInfo?> GetGeoCodeInfoForDutchPostalCodeAndHouseNumber(
            string dutchPostalCode,
            int? houseNumber,
            CancellationToken ct);
    }
}
