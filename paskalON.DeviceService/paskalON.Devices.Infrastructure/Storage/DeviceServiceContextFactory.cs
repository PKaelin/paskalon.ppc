// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace paskalON.Devices.Infrastructure.Storage
{
    /// <summary>
    /// This device service context factory is used for the migration tool ef.
    /// The tool needs to create a database context but we dont want it to start the whole service.
    /// </summary>
    public class DeviceServiceContextFactory : IDesignTimeDbContextFactory<DeviceServiceContext>
    {
        /// <summary>
        /// Create the database context.
        /// </summary>
        /// <param name="args">Possible arguments which are unused.</param>
        /// <returns>The database context.</returns>
        public DeviceServiceContext CreateDbContext(string[] args)
        {
            //-------------------------------------------------------------------
            // This works when ef migration is called from the solution directory
            //-------------------------------------------------------------------
            string? file = Path.Combine(Directory.GetCurrentDirectory(), "..", "secrets", "database_connection");
            string? connectionString = File.ReadAllText(file).Trim();

            ArgumentException.ThrowIfNullOrEmpty(connectionString, "Could not find database connection string.");

            DbContextOptions<DeviceServiceContext> options = new DbContextOptionsBuilder<DeviceServiceContext>()
                    .UseNpgsql(connectionString)
                    .Options;

            return new DeviceServiceContext(options);
        }
    }

}
