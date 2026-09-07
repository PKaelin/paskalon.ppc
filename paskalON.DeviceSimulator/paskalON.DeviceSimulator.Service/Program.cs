// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------

using paskalON.Devices.Client;
using paskalON.Devices.Dto.Ders;

WebApplication? app = null;
Console.WriteLine("Starting service.....");

try
{
    // Get device service endpoint
    Console.WriteLine("Getting environments.....");
    string? getDerEndpointString = Environment.GetEnvironmentVariable("DEVICE_SERVICE_GETDER_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(getDerEndpointString);

    // Create builder
    Console.WriteLine("Building service.....");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add WebApi's
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    app = builder.Build();
    app.Logger.LogInformation("Application has been built");
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => app.Logger.LogInformation("Microservice Device Simulator has started"));
    lifetime.ApplicationStopping.Register(() => app.Logger.LogInformation("Microservice Device Simulator is stopping"));
    app.Logger.LogInformation("Application starts initializing services");

    // Get DER DTO
    IDeviceServer deviceServer = new DeviceServer(getDerEndpointString);
    DerDto derDto = await deviceServer.GetDer();

    app.Logger.LogInformation("Application finished initializing services");

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
    string error = $"Device Simulator startup failed: {ex.Message}";
    // Output to Dockers standard output. Use: docker logs [container]
    Console.WriteLine(error);
    Console.WriteLine(ex.StackTrace);

    if (app != null)
    {
        app.Logger.LogError(error);
        app.Logger.LogError(ex.StackTrace);
    }
}