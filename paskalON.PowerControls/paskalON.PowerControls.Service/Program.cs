// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.PowerControls.Infrastructure.Storage;

WebApplication? app = null;
Console.WriteLine("Starting service.....");

try
{
    Console.WriteLine("Getting environments.....");
    // Get database connection string
    string? dbConnectionStringFile = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_FILE");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dbConnectionStringFile, "Cannot find the database secret file. DATABASE_CONNECTION_FILE");
    string dbConnectionString = (await File.ReadAllTextAsync(dbConnectionStringFile)).Trim();
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dbConnectionString, "Cannot find the database connection string definition");



    // Create builder
    Console.WriteLine("Building service.....");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add WebApi's
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // Add database
    builder.Services.AddDbContext<PowerControlContext>(options => options.UseNpgsql(dbConnectionString));

    // Build application
    app = builder.Build();
    app.Logger.LogInformation("Application has been build");

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    string error = $"PowerControl service startup failed: {ex.Message}";
    // Output to Dockers standard output. Use: docker logs [container]
    Console.Error.WriteLine(error);
    Console.Error.WriteLine(ex.StackTrace);

    if (app != null)
    {
        app.Logger.LogError(error);
        app.Logger.LogError(ex.StackTrace);
    }

    throw;
}
