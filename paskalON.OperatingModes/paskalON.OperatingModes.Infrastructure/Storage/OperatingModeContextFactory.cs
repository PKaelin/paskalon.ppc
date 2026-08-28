// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace paskalON.OperatingModes.Infrastructure.Storage
{
    /// <summary>
    /// This operating mode service context factory is used for the migration tool ef.
    /// The tool needs to create a database context but we dont want it to start the whole service.
    /// </summary>
    public class OperatingModeContextFactory : IDesignTimeDbContextFactory<OperatingModeContext>
    {
        /// <summary>
        /// Create the database context.
        /// </summary>
        /// <param name="args">Possible arguments which are unused.</param>
        /// <returns>The database context.</returns>
        public OperatingModeContext CreateDbContext(string[] args)
        {
            //-------------------------------------------------------------------
            // This works when ef migration is called from the solution directory
            //-------------------------------------------------------------------
            string? file = Path.Combine(Directory.GetCurrentDirectory(), "..", "secrets", "database_connection");
            string? connectionString = File.ReadAllText(file).Trim();

            ArgumentException.ThrowIfNullOrEmpty(connectionString, "Could not find database connection string.");

            DbContextOptions<OperatingModeContext> options = new DbContextOptionsBuilder<OperatingModeContext>()
                    .UseNpgsql(connectionString)
                    .Options;

            return new OperatingModeContext(options);
        }
    }
}
