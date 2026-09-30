// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
WebApplication? app = null;
Console.WriteLine("Starting service.....");

try
{
    Console.WriteLine("Getting environments.....");

    Console.WriteLine("Create builder service.....");
    var builder = WebApplication.CreateBuilder(args);

    // Add WebApi's
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    Console.WriteLine("Building application.....");
    app = builder.Build();
    Console.WriteLine("Application built.....");
    // Register start/stop of the service
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => { app.Logger.LogInformation("Microservice Operating Mode has started."); });
    lifetime.ApplicationStopping.Register(() => { app.Logger.LogInformation("Microservice Operating Mode is stopping."); });
    app.Logger.LogInformation("Application starts initializing services.....");

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
    string errorMessage = $"OperatingMode service startup failed: {ex.Message}";
    Console.WriteLine($"An error occurred: {errorMessage}");
    Console.WriteLine(ex.StackTrace);

    if (app != null)
    {
        app.Logger.LogError(errorMessage);
        app.Logger.LogError(ex.StackTrace);
    }
}