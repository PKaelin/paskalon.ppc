// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Runs the import of a client project file into a database.
    /// </summary>
    /// <remarks>
    /// This is the main entry point for the import of a client project file into a database.
    /// It validates the file and only if it is valid recreates the database and imports the data.
    /// </remarks>
    public static class ClientDataRunner
    {
        /// <summary>
        /// Validates the file and only if it is valid recreates the database and imports the data.
        /// </summary>
        /// <typeparam name="TContext">The database context type of the service.</typeparam>
        /// <param name="args">The program arguments.</param>
        /// <param name="contextFactory">Creates the database context.</param>
        /// <returns>The application exit code.</returns>
        public static async Task<int> RunAsync<TContext>(string[] args, Func<TContext> contextFactory) where TContext : DbContext
        {
            try
            {
                ClientDataArguments arguments = ClientDataArguments.Parse(args);

                if (File.Exists(arguments.ClientProjectFile) is false)
                {
                    throw new ClientDataArgumentException($"ClientProjectFile '{arguments.ClientProjectFile}' does not exist.");
                }

                string[] lines = await File.ReadAllLinesAsync(arguments.ClientProjectFile);
                ClientDataLoader loader = new ClientDataLoader(EntityTypeRegistry.FromContext<TContext>(), DateTimeOffset.UtcNow);
                // Loads the entities from the client project file
                IReadOnlyList<object> entities = loader.Load(arguments.ClientProjectFile, lines);

                await using TContext context = contextFactory();
                string? migration = SchemaVersionResolver.Resolve(arguments.SchemaVersion, context.Database.GetMigrations());

                // Always start from scratch
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync(migration);

                // Adds all the entities to the database and saves the changes
                context.AddRange(entities);
                await context.SaveChangesAsync();

                Console.WriteLine($"Successfully imported {entities.Count} entities of client project '{arguments.ClientProjectName}'.");

                return (int)ClientDataExitCode.Success;
            }
            catch (ClientDataArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine(ClientDataArguments.Usage);

                return (int)ClientDataExitCode.InvalidArguments;
            }
            catch (ClientDataValidationException ex)
            {
                Console.Error.WriteLine(ex.Message);

                return (int)ClientDataExitCode.CsvValidationError;
            }
            catch (Exception ex) when (ex is DbException or DbUpdateException)
            {
                Console.Error.WriteLine($"Database error: {ex.Message}");

                return (int)ClientDataExitCode.DatabaseError;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error: {ex.Message}");

                return (int)ClientDataExitCode.GeneralError;
            }
        }
    }
}
