// <copyright file="BackendOptionsValidator.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Common.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fvect.Backend.Common.Models.Geo;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Represents a validator for the application options.
    /// </summary>
    public class BackendOptionsValidator : IValidateOptions<BackendOptions>
    {
        /// <inheritdoc />
        public ValidateOptionsResult Validate(string name, BackendOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));

            var faults = new List<string>();

            if (ValidateNotNull(options.Database, nameof(options.Database), faults))
            {
                var prefix = $"{nameof(options.Database)}.";
                var section = options.Database;

                ValidateStringNotNullOrWhitespace(
                    section.SQLServerConnectionString,
                    $"{prefix}{nameof(section.SQLServerConnectionString)}",
                    faults);
            }

            if (ValidateNotNull(options.Geo, nameof(options.Geo), faults))
            {
                var prefix = $"{nameof(options.Geo)}.";
                var section = options.Geo;

                ValidateStringNotNullOrWhitespace(
                    section.HereMapsAPIKey,
                    $"{prefix}{nameof(section.HereMapsAPIKey)}",
                    faults);

                ValidateUri(
                    section.HEREGeocodeServiceBaseUri,
                    $"{prefix}{nameof(section.HEREGeocodeServiceBaseUri)}",
                    faults,
                    Uri.UriSchemeHttp,
                    Uri.UriSchemeHttps);

                ValidateUri(
                    section.HEREMapImageServiceBaseUri,
                    $"{prefix}{nameof(section.HEREMapImageServiceBaseUri)}",
                    faults,
                    Uri.UriSchemeHttp,
                    Uri.UriSchemeHttps);

                ValidateTimeSpan(
                    section.MapImageCacheEvictionInterval,
                    $"{prefix}{nameof(section.MapImageCacheEvictionInterval)}",
                    faults);

                ValidateInteger(
                    section.DefaultResponseHeightForMapImage,
                    $"{prefix}{nameof(section.DefaultResponseHeightForMapImage)}",
                    faults,
                    lowerBound: 1);

                ValidateInteger(
                    section.DefaultResponseWidthForMapImage,
                    $"{prefix}{nameof(section.DefaultResponseWidthForMapImage)}",
                    faults,
                    lowerBound: 1);

                ValidateInteger(
                    section.MaxRequestHeightForMapImage,
                    $"{prefix}{nameof(section.MaxRequestHeightForMapImage)}",
                    faults,
                    lowerBound: 1);

                ValidateInteger(
                    section.MaxRequestWidthForMapImage,
                    $"{prefix}{nameof(section.MaxRequestWidthForMapImage)}",
                    faults,
                    lowerBound: 1);

                ValidateInteger(
                    section.HERERequestHeightForMapImage,
                    $"{prefix}{nameof(section.HERERequestHeightForMapImage)}",
                    faults,
                    lowerBound: 1);

                ValidateInteger(
                    section.HERERequestWidthForMapImage,
                    $"{prefix}{nameof(section.HERERequestWidthForMapImage)}",
                    faults,
                    lowerBound: 1);

                if (ValidateNotNull(
                    section.CacheTimesPerZoomLevel,
                    $"{prefix}{nameof(section.CacheTimesPerZoomLevel)}",
                    faults))
                {
                    foreach (var (key, value) in section.CacheTimesPerZoomLevel)
                    {
                        ValidateEnum(
                            key,
                            $"{prefix}{nameof(section.CacheTimesPerZoomLevel)}.{key}",
                            faults);

                        ValidateTimeSpan(
                            value,
                            $"{prefix}{nameof(section.CacheTimesPerZoomLevel)}.{key}",
                            faults,
                            mayBeZero: true);
                    }

                    foreach (var enumValue in Enum.GetValues(typeof(ZoomLevel)).Cast<ZoomLevel>())
                    {
                        if (!section.CacheTimesPerZoomLevel.ContainsKey(enumValue))
                        {
                            faults.Add(
                                $"\'{prefix}{nameof(section.CacheTimesPerZoomLevel)}\' misses a value for key \'{enumValue}\'.");
                        }
                    }
                }
            }

            if (ValidateNotNull(options.AuthNR, nameof(options.AuthNR), faults))
            {
                var prefix = $"{nameof(options.AuthNR)}.";
                var section = options.AuthNR;

                ValidateStringNotNullOrWhitespace(
                    section.JWTAudience,
                    $"{prefix}{nameof(section.JWTAudience)}",
                    faults);

                ValidateStringNotNullOrWhitespace(
                    section.JWTAuthority,
                    $"{prefix}{nameof(section.JWTAuthority)}",
                    faults);

                if (ValidateStringNotNullOrWhitespace(
                    section.JWTSigningKey,
                    $"{prefix}{nameof(section.JWTSigningKey)}",
                    faults))
                {
                    if (section.JWTSigningKey.Length < 16)
                    {
                        faults.Add(
                            $"\'{prefix}{nameof(section.JWTSigningKey)}\' must be at least 16 characters long.");
                    }
                }
            }

            if (faults.Count > 0)
            {
                return ValidateOptionsResult.Fail(faults);
            }

            return ValidateOptionsResult.Success;
        }

        /// <summary>
        /// Validates that a string setting is not null, empty or whitespace, and appends
        /// a fault message to a collection when the string setting is null, empty or whitespace.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="faultCollection">The fault collection.</param>
        /// <returns>A value indicating whether validation succeeded or not.</returns>
        private static bool ValidateStringNotNullOrWhitespace(
            string? value,
            string settingName,
            List<string> faultCollection)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                faultCollection.Add($"\'{settingName}\' may not be null, empty or whitespace.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates that a complex-type setting is not null, and appends
        /// a fault message to a collection when the setting is null.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="faultCollection">The fault collection.</param>
        /// <returns>A value indicating whether validation succeeded or not.</returns>
        private static bool ValidateNotNull<TValue>(
            TValue? value,
            string settingName,
            List<string> faultCollection)
            where TValue : class
        {
            if (value is null)
            {
                faultCollection.Add($"\'{settingName}\' may not be null.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates that a value-type setting is not null, and appends
        /// a fault message to a collection when the setting is null.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="faultCollection">The fault collection.</param>
        /// <returns>A value indicating whether validation succeeded or not.</returns>
        private static bool ValidateNotNull<TValue>(
            TValue? value,
            string settingName,
            List<string> faultCollection)
            where TValue : struct
        {
            if (value is null)
            {
                faultCollection.Add($"\'{settingName}\' may not be null.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates a <see cref="Uri"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="faultCollection">The fault collection.</param>
        /// <param name="allowedSchemes">The allowed values for <see cref="Uri.Scheme"/>.</param>
        /// <returns>A value indicating whether validation succeeded or not.</returns>
        private static bool ValidateUri(
            Uri? value,
            string settingName,
            List<string> faultCollection,
            params string[] allowedSchemes)
        {
            if (!ValidateNotNull(value, settingName, faultCollection))
            {
                return false;
            }

            if (!ValidateStringNotNullOrWhitespace(
                value!.Host,
                $"{settingName}.{nameof(value.Host)}",
                faultCollection))
            {
                return false;
            }

            if (!allowedSchemes.Any(x => x == value!.Scheme))
            {
                faultCollection.Add($"\'{settingName}.{nameof(value.Scheme)}\' has a value that is not allowed.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="settingName">The name of the setting being validated.</param>
        /// <param name="faultCollection">The collection of error messages.</param>
        /// <param name="mayBeNull">A value indicating whether the <see cref="TimeSpan"/> may be <c>null</c>.</param>
        /// <param name="mayBeNegative">A value indicating whether the <see cref="TimeSpan"/> may be negative,
        /// that is, below <see cref="TimeSpan.Zero"/>.
        /// </param>
        /// <param name="mayBeZero">
        /// A value indicating whether the <see cref="TimeSpan" /> may be
        /// equal to <see cref="TimeSpan.Zero"/>.
        /// </param>
        /// <param name="mayBePositive">
        /// A value indicating whether the timespan may be
        /// greater then zero.
        /// </param>
        /// <returns>
        /// A value indiciating if validation succeeded.
        /// </returns>
        private static bool ValidateTimeSpan(
            TimeSpan? value,
            string settingName,
            List<string> faultCollection,
            bool mayBeNull = false,
            bool mayBeNegative = false,
            bool mayBeZero = false,
            bool mayBePositive = true)
        {
            if (value is null && mayBeNull)
            {
                return true;
            }

            if (!ValidateNotNull(
                value,
                settingName,
                faultCollection))
            {
                return false;
            }

            if (value < TimeSpan.Zero && !mayBeNegative)
            {
                faultCollection.Add(
                    $"\'{settingName}\' may not be negative.");
                return false;
            }

            if (value == TimeSpan.Zero && !mayBeZero)
            {
                faultCollection.Add(
                    $"\'{settingName}\' may not be zero.");
                return false;
            }

            if (value > TimeSpan.Zero && !mayBePositive)
            {
                faultCollection.Add(
                    $"\'{settingName}\' may not be positive.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates an enumeration.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="settingName">The name of the setting being validated.</param>
        /// <param name="faultCollection">The collection of error messages.</param>
        /// <returns>A value indicating whether validation succeeded.</returns>
        /// <remarks>
        /// Does not work for flag enums.
        /// </remarks>
        private static bool ValidateEnum<TEnum>(
            TEnum value,
            string settingName,
            List<string> faultCollection)
            where TEnum : Enum
        {
            if (typeof(TEnum).CustomAttributes.Any(x => x.AttributeType.Equals(typeof(FlagsAttribute))))
            {
                throw new InvalidOperationException("Cannot be used to validate flag enums.");
            }

            if (!Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Contains(value))
            {
                faultCollection.Add(
                    $"\'{settingName}\' is not a valid value for type \'{typeof(TEnum).Name}\'.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates an <see cref="int"/>.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="settingName">The name of the setting that is being validated.</param>
        /// <param name="faultCollection">The fault collection.</param>
        /// <param name="mayBeNull">A value indicating whether the value may be null.</param>
        /// <param name="lowerBound">The exclusive lower bound.</param>
        /// <param name="upperBound">The exclusive upper bound.</param>
        /// <returns>A value indicating if the validation was successfull.</returns>
        private static bool ValidateInteger(
            int? value,
            string settingName,
            List<string> faultCollection,
            bool mayBeNull = false,
            int? lowerBound = 0,
            int? upperBound = null)
        {
            if (value is null && mayBeNull)
            {
                return true;
            }

            if (!ValidateNotNull(
                value,
                settingName,
                faultCollection))
            {
                return false;
            }

            if (lowerBound != null && value < lowerBound)
            {
                faultCollection.Add(
                    $"\'{settingName}\' must be greater then or equal to {lowerBound}.");
                return false;
            }

            if (upperBound != null && value > upperBound)
            {
                faultCollection.Add(
                    $"\'{settingName}\' must be smaller then or equal to {upperBound}.");
                return false;
            }

            return true;
        }
    }
}
