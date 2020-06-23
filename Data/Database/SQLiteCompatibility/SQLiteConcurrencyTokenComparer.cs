// <copyright file="SQLiteConcurrencyTokenComparer.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Database.SQLiteCompatibility
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    /// <summary>
    /// Represents a value converter for concurrency tokens used with SQLite.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.SpacingRules",
        "SA1009:Closing parenthesis should be spaced correctly",
        Justification = "False positive. Ref: https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3143.")]
    [SuppressMessage(
        "StyleCop.CSharp.SpacingRules",
        "SA1011:Closing square brackets should be spaced correctly",
        Justification = "False positive. Ref: https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3143.")]
    public class SQLiteConcurrencyTokenComparer : ValueComparer<byte[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteConcurrencyTokenComparer"/> class.
        /// </summary>
        public SQLiteConcurrencyTokenComparer()
            : base((a, b) => DoEquals(a, b), (i) => DoHashCode(i), (i) => DoSnapshot(i)!)
        {
        }

        private static bool DoEquals(byte[]? a, byte[]? b) =>
            a == null && b == null ? true : a == null && b != null ? false : a != null && b == null ? false
                : Encoding.UTF8.GetString(a).Equals(Encoding.UTF8.GetString(b), StringComparison.Ordinal);

        private static int DoHashCode(byte[]? input) =>
            input != null ? Encoding.UTF8.GetString(input).GetHashCode(StringComparison.Ordinal) : 0;

        private static byte[]? DoSnapshot(byte[]? input) =>
            input != null ? (byte[])input.Clone() : null;
    }
}
