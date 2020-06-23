// <copyright file="ZoomLevel.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Models.Geo
{
    /// <summary>
    /// Represents the zoom level for a requested map.
    /// </summary>
    public enum ZoomLevel
    {
        /// <summary>
        /// The map is zoomed in to a single house.
        /// </summary>
        HOUSE = 0,

        /// <summary>
        /// The map is zoomed in to a single street.
        /// </summary>
        STREET = 1,

        /// <summary>
        /// The map is zoomed in to a neighbourhood.
        /// </summary>
        NEIGHBOURHOOD = 2,

        /// <summary>
        /// The map is zoomed in to a city.
        /// </summary>
        CITY = 3,

        /// <summary>
        /// The map is zoomed in to a municipality.
        /// </summary>
        MUNICIPALITY = 4,

        /// <summary>
        /// The map is zoomed in to a province.
        /// </summary>
        PROVINCE = 5,
    }
}
