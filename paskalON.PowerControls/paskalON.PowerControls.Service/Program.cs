// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.Devices.Client;
using paskalON.Infrastructure.Repositories;
using paskalON.Messaging;
using paskalON.Messaging.Redis;
using paskalON.PowerControls.Application;
using paskalON.PowerControls.Domain.Configs;
using paskalON.PowerControls.Infrastructure.Storage;
using paskalON.PowerControls.Infrastructure.Storage.Repositories;
using paskalON.PowerControls.Service.Publishers;
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
    string dsConnectionString = (await File.ReadAllTextAsync(dsConnectionStringFile)).Trim();
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dsConnectionString, "Cannot find the device service connection string definition");

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

    // Create builder
    Console.WriteLine("Building service.....");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add WebApi's
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // Add database
    builder.Services.AddDbContext<PowerControlContext>(options => options.UseNpgsql(dbConnectionString));
    builder.Services.AddScoped<IVersionRepository, VersionRepository>();
    builder.Services.AddScoped<IPowerControlRepository, PowerControlRepository>();
    builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

    // Add communications
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(msgConnectionString));
    builder.Services.AddSingleton<IMetricsPublisherFactory, MetricsPublisherFactory>();
    builder.Services.AddTransient<IMetricsPublisher, MetricsPublisher>();
    builder.Services.AddSingleton<MetricsPublisherService>();
    builder.Services.AddSingleton<IDeviceServer>(sp => new DeviceServer(dsConnectionString));
    builder.Services.AddSingleton<IMessageSubscriber, RedisMessageSubscriber>();
    builder.Services.AddSingleton<IDeviceClient, DeviceClient>();

    builder.Services.AddSingleton<IPowerControlManager, PowerControlManager>();

    if (startPublisher == true)
    {
        builder.Services.AddHostedService<MetricsPublisherService>(provider => provider.GetRequiredService<MetricsPublisherService>());
    }

    // Build application
    Console.WriteLine("Building application.....");
    app = builder.Build();
    app.Logger.LogInformation("Application built.....");
    // Register start/stop of the service
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => { app.Logger.LogInformation("Microservice Power Controls has started"); });
    lifetime.ApplicationStopping.Register(() => { app.Logger.LogInformation("Microservice Power Controls is stopping"); });
    app.Logger.LogInformation("Application starts initializing services.....");
    // Create and load device manager
    IPowerControlManager manager = app.Services.GetRequiredService<IPowerControlManager>();

    using (IServiceScope scope = app.Services.CreateScope())
    {
        IRepository<PowerControlContext, SystemConfig> repository = scope.ServiceProvider.GetRequiredService<IRepository<PowerControlContext, SystemConfig>>();
        SystemConfig? config = repository.GetAsync(0, 1, (o) => o.Id).Result.FirstOrDefault();
        ArgumentNullException.ThrowIfNull(config, "System configuration contains no record");

        if (startPublisher == true)
        {
            MetricsPublisherService metricsPublisherService = app.Services.GetRequiredService<MetricsPublisherService>();
            metricsPublisherService.Initialize(manager.MetricsPublishers, config.MetricsIntervalMilliseconds, config.StartupDelayForDevices);
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
