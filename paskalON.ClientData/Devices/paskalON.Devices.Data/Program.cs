// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using paskalON.ClientData.Common;
using paskalON.Devices.Infrastructure.Storage;

/// <summary>
/// Imports a client project file into the device service database.
/// </summary>
class Program
{
    /// <summary>
    /// Entry point of the import.
    /// </summary>
    /// <param name="args">ClientProjectName, ClientProjectFile and optional SchemaVersion.</param>
    /// <returns>The value of the <see cref="ClientDataExitCode"/>.</returns>
    /// <remarks>
    /// Arguments for DemoSuperSimpleBattery:
    /// Argument 1: "DemoSuperSimpleBattery"
    /// Argument 2: "ClientData\Demo\DemoSuperSimpleBattery.csv" (the relative path is resolved from the working directory which has to be the output directory)
    /// Argument 3: "20261020230224_v_0_1"  (Optional schema version found in infrastructure migration folder or null to use the latest version.
    /// Important: The database in appsettings.json is deleted and recreated if the arguments and the file are valid.
    /// Set debug arguments in Visual Studio: Project Properties -> Debug -> Application arguments
    /// </remarks>
    public static async Task<int> Main(string[] args)
    {
        return await ClientDataRunner.RunAsync(args, CreateContext);
    }


    /// <summary>
    /// Creates the database context from the application settings.
    /// </summary>
    /// <returns>The database context.</returns>
    private static DeviceServiceContext CreateContext()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string? connectionString = configuration.GetConnectionString("DatabaseContext");
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        DbContextOptions<DeviceServiceContext> options = new DbContextOptionsBuilder<DeviceServiceContext>().UseNpgsql(connectionString).Options;

        return new DeviceServiceContext(options);
    }
}
