// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.Devices.Application;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Infrastructure.Storage;
using paskalON.Devices.Infrastructure.Storage.Repositories;
using paskalON.DeviceSimulator.Application;
using paskalON.DeviceSimulator.Application.Factories;
using paskalON.DeviceSimulator.Application.Simulations;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems;
using paskalON.DeviceSimulator.Service.Workers;
using paskalON.Telemetry;

WebApplication? app = null;
Console.WriteLine("Starting service.....");

try
{
    Console.WriteLine("Getting environments.....");
    string? dbConnectionStringFile = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_FILE");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dbConnectionStringFile, "Cannot find the database secret file. DATABASE_CONNECTION_FILE");
    string dbConnectionString = (await File.ReadAllTextAsync(dbConnectionStringFile)).Trim();
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dbConnectionString, "Cannot find the database connection string definition");
    // Get data rate
    string? dataRateString = Environment.GetEnvironmentVariable("PMU_DATA_RATE");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dataRateString);
    if (ushort.TryParse(dataRateString, out ushort dataRate) == false)
    {
        throw new ApplicationException("PMU_DATA_RATE is not configured as a number");
    }
    // Get simulation interval
    string? simulationIntervalString = Environment.GetEnvironmentVariable("SIMULATION_INTERVAL_MILLISECONDS");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(simulationIntervalString);
    if (ushort.TryParse(simulationIntervalString, out ushort simulationInterval) == false)
    {
        throw new ApplicationException("SIMULATION_INTERVAL_MILLISECONDS is not configured as a number");
    }

    // Create builder
    Console.WriteLine("Building service.....");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add WebApi's
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // Add databases
    builder.Services.AddDbContext<DeviceServiceContext>(options => options.UseNpgsql(dbConnectionString));
    builder.Services.AddScoped<IDerRepository, DerRepository>();
    builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

    // Add communications
    builder.Services.AddTransient<IMetricsPublisher, MetricsPublisher>();
    builder.Services.AddSingleton<IMetricsPublisherFactory, MetricsPublisherFactory>();
    builder.Services.AddSingleton<ISimulationStoreRegistry, SimulationStoreRegistry>();
    builder.Services.AddSingleton<SimulationDeviceRegistry>();
    builder.Services.AddSingleton<ISimulationModelFactory, PowerConversionSystemSimulationModelFactory>();
    builder.Services.AddSingleton<IModbusDeviceFactory, SimulationModbusDeviceFactory>();
    builder.Services.AddSingleton<IC37DeviceFactory, C37DeviceFactory>();
    builder.Services.AddSingleton<IDeviceManager, DeviceManagerSimulator>();

    // Add simulations
    builder.Services.AddSingleton<SimulationWorker>();
    builder.Services.AddHostedService<SimulationWorker>();

    // Build application
    app = builder.Build();
    app.Logger.LogInformation("Application has been built");
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => app.Logger.LogInformation("Microservice Device Simulator has started"));
    lifetime.ApplicationStopping.Register(() => app.Logger.LogInformation("Microservice Device Simulator is stopping"));
    app.Logger.LogInformation("Application starts initializing services");
    // Create and load device manager
    DeviceManagerSimulator deviceManager = (DeviceManagerSimulator)app.Services.GetRequiredService<IDeviceManager>();
    deviceManager.Initialize(dataRate, lifetime.ApplicationStopping);

    using (IServiceScope scope = app.Services.CreateScope())
    {
        IDerRepository repository = scope.ServiceProvider.GetRequiredService<IDerRepository>();
        await deviceManager.LoadDerAsync(repository);
    }

    // Initialize simulation service
    SimulationWorker simulationService = app.Services.GetRequiredService<SimulationWorker>();
    simulationService.Initialize(simulationInterval);

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