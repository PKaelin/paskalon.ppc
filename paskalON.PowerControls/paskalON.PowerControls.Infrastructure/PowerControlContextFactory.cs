// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using paskalON.PowerControls.Infrastructure.Storage;

namespace paskalON.PowerControls.Infrastructure
{
    public class PowerControlContextFactory : IDesignTimeDbContextFactory<PowerControlContext>
    {
        /// <summary>
        /// Create the database context.
        /// </summary>
        /// <param name="args">Possible arguments which are unused.</param>
        /// <returns>The database context.</returns>
        public PowerControlContext CreateDbContext(string[] args)
        {
            //-------------------------------------------------------------------
            // This works when ef migration is called from the solution directory
            //-------------------------------------------------------------------
            string? file = Path.Combine(Directory.GetCurrentDirectory(), "..", "secrets", "database_connection");
            string? connectionString = File.ReadAllText(file).Trim();

            ArgumentException.ThrowIfNullOrEmpty(connectionString, "Could not find database connection string.");

            DbContextOptions<PowerControlContext> options = new DbContextOptionsBuilder<PowerControlContext>()
                    .UseNpgsql(connectionString)
                    .Options;

            return new PowerControlContext(options);
        }
    }
}
