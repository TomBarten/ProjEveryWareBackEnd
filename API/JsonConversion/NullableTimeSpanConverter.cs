// <copyright file="NullableTimeSpanConverter.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.API.JsonConversion
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Xml;

    /// <summary>
    /// Represents a <see cref="JsonConverter"/> for nullable <see cref="TimeSpan"/> instances.
    /// </summary>
    public class NullableTimeSpanConverter : JsonConverter<TimeSpan?>
    {
        /// <inheritdoc />
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Null
                ? null as TimeSpan?
                : XmlConvert.ToTimeSpan(reader.GetString());

        /// <inheritdoc />
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Performance")]
        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(XmlConvert.ToString(value.Value));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
