// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------

// Name of this service in the heartbeat and the source instance of the published messages
const string serviceName = "Alarm";
WebApplication? app = null;
Console.WriteLine("Starting service.....");

try
{
    Console.WriteLine("Create builder service.....");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add WebApi's
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    Console.WriteLine("Building application.....");
    app = builder.Build();
    Console.WriteLine("Application built.....");
    // Register start/stop of the service
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => { app.Logger.LogInformation("Microservice Alarm has started."); });
    lifetime.ApplicationStopping.Register(() => { app.Logger.LogInformation("Microservice Alarm is stopping."); });

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseAuthorization();
    app.MapControllers();
    app.Logger.LogInformation("Application about to run.....");
    app.Run();
}
catch (Exception ex)
{
    string errorMessage = $"{serviceName} service startup failed: {ex.Message}";
    Console.WriteLine($"An error occurred: {errorMessage}");
    Console.WriteLine(ex.StackTrace);

    if (app != null)
    {
        app.Logger.LogError(errorMessage);
        app.Logger.LogError(ex.StackTrace);
    }

    // Fail application
    throw;
}
