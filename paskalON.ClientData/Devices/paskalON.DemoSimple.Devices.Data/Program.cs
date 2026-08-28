// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using paskalON.DemoSimple.Devices.Data;
using paskalON.Devices.Infrastructure.Storage;

/// <summary>
/// Program class for data handling.
/// </summary>
/// <remarks>
/// Either checkout latest version of this code or specific version.
/// Whatever version is checked out the SchemaVersion has to be aligned.
/// </remarks>
class Program
{
    /// <summary>
    /// Entry point for data handling.
    /// </summary>
    /// <param name="args">Arguments for program control.</param>
    /// <remarks>
    /// Don't do any exception handling.
    /// </remarks>
    public static void Main(string[] args)
    {
        // Setup configuration
        IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        // Read arguments or configuration
        // Use specific: "SchemaVersion": "20260530987654_v_0_1"
        // Use latest:   "SchemaVersion": ""
        string? schemaVersion = args.Length > 0 ? args[0] : configuration.GetSection("SchemaVersion")?.Value;

        // Read database connection string and create db context options
        string? connectionString = configuration.GetConnectionString("DatabaseContext");
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        DbContextOptions<DeviceServiceContext> options = new DbContextOptionsBuilder<DeviceServiceContext>().UseNpgsql(connectionString).Options;

        using (DeviceServiceContext context = new DeviceServiceContext(options))
        {
            // Always start from scratch
            context.Database.EnsureDeleted();

            // ----------------------------------------------------------------------------
            // Important to be able to migrate to a version a migration entry has to exist:
            // [Microservice].Infrastructure.Storage.Migrations
            // See README.md in [Microservice].Infrastructure\README.md
            // ----------------------------------------------------------------------------
            if (string.IsNullOrEmpty(schemaVersion) == false)
            {
                // Migrate the database to a specific version
                context.Database.Migrate(schemaVersion);
            }
            else
            {
                // Migrate the database to the latest version
                context.Database.Migrate();
            }

            // Create and save the data
            CommonData.Create(context);
            ServiceData.Create(context);
            context.SaveChanges();
        }
    }
}
