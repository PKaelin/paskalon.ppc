// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace paskalON.Alarms.Infrastructure.Storage
{
    /// <summary>
    /// This alarm service context factory is used for the migration tool ef.
    /// The tool needs to create a database context but we dont want it to start the whole service.
    /// </summary>
    public class AlarmContextFactory : IDesignTimeDbContextFactory<AlarmContext>
    {
        /// <summary>
        /// Create the database context.
        /// </summary>
        /// <param name="args">Possible arguments which are unused.</param>
        /// <returns>The database context.</returns>
        public AlarmContext CreateDbContext(string[] args)
        {
            //-------------------------------------------------------------------
            // This works when ef migration is called from the solution directory
            //-------------------------------------------------------------------
            string file = Path.Combine(Directory.GetCurrentDirectory(), "..", "secrets", "database_connection");
            string connectionString = File.ReadAllText(file).Trim();

            ArgumentException.ThrowIfNullOrEmpty(connectionString, "Could not find database connection string.");

            DbContextOptions<AlarmContext> options = new DbContextOptionsBuilder<AlarmContext>()
                    .UseNpgsql(connectionString)
                    .Options;

            return new AlarmContext(options);
        }
    }
}
