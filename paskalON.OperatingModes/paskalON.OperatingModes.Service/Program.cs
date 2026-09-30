// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.Infrastructure.Repositories;
using paskalON.OperatingModes.Application;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Infrastructure.Storage;
using paskalON.OperatingModes.Infrastructure.Storage.Repositories;
using paskalON.OperatingModes.Service.Publishers;
using paskalON.Telemetry;
using paskalON.Telemetry.Factories;
using StackExchange.Redis;

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

    // Get messaging connection string
    string? msgConnectionStringFile = Environment.GetEnvironmentVariable("MESSAGING_CONNECTION_FILE");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(msgConnectionStringFile, "Cannot find the messaging secret file. MESSAGING_CONNECTION_FILE");
    string msgConnectionString = (await File.ReadAllTextAsync(msgConnectionStringFile)).Trim();
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(msgConnectionString, "Cannot find the messaging connection string definition");

    // Get device service connection string
    string? dsConnectionStringFile = Environment.GetEnvironmentVariable("DEVICESERVICE_CONNECTION_FILE");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dsConnectionStringFile, "Cannot find the device service secret file. DEVICESERVICE_CONNECTION_FILE");
    string dsConnectionStringArray = (await File.ReadAllTextAsync(dsConnectionStringFile)).Trim();
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dsConnectionStringArray, "Cannot find the device service connection string definition");
    string[] dsConnectionStrings = dsConnectionStringArray.Split(';', StringSplitOptions.RemoveEmptyEntries);
    // Get device service DER endpoint
    string? dsEndpoint = Environment.GetEnvironmentVariable("DEVICE_SERVICE_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dsEndpoint, "Cannot find the device service endpoint. DEVICE_SERVICE_ENDPOINT");

    // Get Logging, Metrics, Tracing endpoint strings
    string? logEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_LOGGING_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(logEndpointString, "Cannot find the logging endpoint. TELEMETRY_LOGGING_ENDPOINT");
    string? metricsEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_METRICS_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(metricsEndpointString, "Cannot find the metrics endpoint. TELEMETRY_METRICS_ENDPOINT");
    string? tracingEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_TRACING_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(tracingEndpointString, "Cannot find the tracing endpoint. TELEMETRY_TRACING_ENDPOINT");

    // Get start options
    string startPublisherString = Environment.GetEnvironmentVariable("START_PUBLISHERS") ?? "true";
    if (bool.TryParse(startPublisherString, out var startPublisher) == false)
    {
        throw new ApplicationException("START_PUBLISHERS is not configured as a boolean");
    }

    Console.WriteLine("Create builder service.....");
    var builder = WebApplication.CreateBuilder(args);

    // Add WebApi's
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // Add database
    builder.Services.AddDbContext<OperatingModeContext>(options => options.UseNpgsql(dbConnectionString));
    builder.Services.AddScoped<IVersionRepository, VersionRepository>();
    builder.Services.AddScoped<IOperatingModeRepository, OperatingModeRepository>();
    builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

    // Add communications
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(msgConnectionString));
    builder.Services.AddSingleton<IMetricsPublisherFactory, MetricsPublisherFactory>();
    builder.Services.AddTransient<IMetricsPublisher, MetricsPublisher>();
    builder.Services.AddSingleton<MetricsPublisherService>();

    builder.Services.AddSingleton<IOperatingModeManager, OperatingModeManager>();

    if (startPublisher == true)
    {
        builder.Services.AddHostedService<MetricsPublisherService>(provider => provider.GetRequiredService<MetricsPublisherService>());
    }

    Console.WriteLine("Building application.....");
    app = builder.Build();
    Console.WriteLine("Application built.....");
    // Register start/stop of the service
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => { app.Logger.LogInformation("Microservice Operating Mode has started."); });
    lifetime.ApplicationStopping.Register(() => { app.Logger.LogInformation("Microservice Operating Mode is stopping."); });
    app.Logger.LogInformation("Application starts initializing services.....");
    // Create and load device manager
    IOperatingModeManager manager = app.Services.GetRequiredService<IOperatingModeManager>();
    // Create and load manager
    using (IServiceScope scope = app.Services.CreateScope())
    {
        IRepository<OperatingModeContext, SystemConfig> repository = scope.ServiceProvider.GetRequiredService<IRepository<OperatingModeContext, SystemConfig>>();
        SystemConfig config = (await repository.GetAsync(0, 1, o => o.Id)).Single();

        if (startPublisher == true)
        {
            MetricsPublisherService metricsPublisherService = app.Services.GetRequiredService<MetricsPublisherService>();
            metricsPublisherService.Initialize(manager.MetricsPublishers, config.MetricsIntervalMilliseconds, config.StartupDelay);
        }
    }

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