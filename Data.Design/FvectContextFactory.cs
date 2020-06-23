// <copyright file="FvectContextFactory.cs" company="Fvect">
// Copyright (c)
//  Aaron Slots, Arthur Heidt, Jeroen de Klerk, Luc van Dijk,
//  Ryan van Gemert, Sjanne Flinterman, Thomas Maassen and Tom Barten.
// All rights reserved.
// </copyright>

namespace Fvect.Backend.Data.Design
{
    using Fvect.Backend.Data.Database;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    /// <summary>
    /// Represents a factory for an <see cref="FvectContext"/> that is
    /// intended to be used at design-time.
    /// </summary>
    public class FvectContextFactory : IDesignTimeDbContextFactory<FvectContext>
    {
        private static readonly string DesignDatabaseConnectionString = @"Server=localhost;Database=FVectLocal;Trusted_Connection=True;";

        /// <inheritdoc />
        public FvectContext CreateDbContext(string[] args) =>
            new FvectContext(
                new DbContextOptionsBuilder<FvectContext>()
                    .UseSqlServer(DesignDatabaseConnectionString)
                    .Options);
    }
}
