// <copyright file="SQLiteConcurrencyTokenConverter.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.SQLiteCompatibility
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

    /// <summary>
    /// Represents a value converter for concurrency tokens between the CLR and
    /// SQLite.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.SpacingRules",
        "SA1009:Closing parenthesis should be spaced correctly",
        Justification = "False positive. Ref: https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3143.")]
    [SuppressMessage(
        "StyleCop.CSharp.SpacingRules",
        "SA1011:Closing square brackets should be spaced correctly",
        Justification = "False positive. Ref: https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3143.")]
    public class SQLiteConcurrencyTokenConverter : ValueConverter<byte[], string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteConcurrencyTokenConverter"/> class.
        /// </summary>
        public SQLiteConcurrencyTokenConverter()
            : base(
                  (v) => ToDb(v)!,
                  (v) => FromDb(v)!)
        {
        }

        private static byte[]? FromDb(string? v) =>
            v != null ? Encoding.UTF8.GetBytes(v) : null;

        private static string? ToDb(byte[]? v) =>
            v != null ? Encoding.UTF8.GetString(v) : null;
    }
}
