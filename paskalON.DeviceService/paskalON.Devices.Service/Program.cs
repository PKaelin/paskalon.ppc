// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using paskalON.Devices.Application;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Application.Publishers;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Infrastructure.Storage;
using paskalON.Devices.Infrastructure.Storage.Repositories;
using paskalON.Devices.Service.Publishers;
using paskalON.Devices.Service.Workers;
using paskalON.Messaging;
using paskalON.Messaging.Redis;
using paskalON.Telemetry;
using StackExchange.Redis;

WebApplication? app = null;

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

    // Get Logging, Metrics, Tracing endpoint strings
    string? logEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_LOGGING_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(logEndpointString, "Cannot find the logging endpoint. TELEMETRY_LOGGING_ENDPOINT");
    string? metricsEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_METRICS_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(metricsEndpointString, "Cannot find the metrics endpoint. TELEMETRY_METRICS_ENDPOINT");
    string? tracingEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_TRACING_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(tracingEndpointString, "Cannot find the tracing endpoint. TELEMETRY_TRACING_ENDPOINT");

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
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(msgConnectionString));
    builder.Services.AddSingleton<IMessagePublisher, RedisMessagePublisher>();
    builder.Services.AddSingleton<IModbusDeviceFactory, ModbusDeviceFactory>();
    builder.Services.AddSingleton<IC37DeviceFactory, C37DeviceFactory>();
    builder.Services.AddSingleton<IMetricsPublisherFactory, MetricsPublisherFactory>();
    builder.Services.AddTransient<IMetricsPublisher, MetricsPublisher>();
    builder.Services.AddSingleton<ModbusPollService>();
    builder.Services.AddHostedService<ModbusPollService>(provider => provider.GetRequiredService<ModbusPollService>());
    builder.Services.AddSingleton<DevicePublisherService>();
    builder.Services.AddHostedService<DevicePublisherService>(provider => provider.GetRequiredService<DevicePublisherService>());
    builder.Services.AddSingleton<MetricsPublisherService>();
    builder.Services.AddHostedService<MetricsPublisherService>(provider => provider.GetRequiredService<MetricsPublisherService>());
    builder.Services.AddSingleton<IDeviceManager, DeviceManager>();

    // Configure OpenTelemetry logging, metrics, & tracing with auto-start using the
    // AddOpenTelemetry extension from OpenTelemetry.Extensions.Hosting.
    builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r
        .AddService(
            serviceName: builder.Environment.ApplicationName,
            serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown",
            serviceInstanceId: Environment.MachineName))
        .WithLogging(builder =>
        {
            builder.AddOtlpExporter((otlpOptions, logRecordExportProcessorOptions) =>
            {
                otlpOptions.Endpoint = new Uri(logEndpointString);
                otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });
        })
        .WithMetrics(builder =>
        {
            builder.AddAspNetCoreInstrumentation();
            builder.AddMeter("Solar");
            builder.AddMeter("BMS");
            builder.AddMeter("GMD");
            builder.AddMeter("PowerMeter");
            builder.AddMeter("PCS");

            builder.AddOtlpExporter((otlpOptions, metricReaderOptions) =>
            {
                otlpOptions.Endpoint = new Uri(metricsEndpointString);
                otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000;
            });
        })
        .WithTracing(builder =>
        {
            builder.AddAspNetCoreInstrumentation();
            builder.AddSource("PPC.Devices");
            builder.AddOtlpExporter((otlpOptions) =>
            {
                otlpOptions.Endpoint = new Uri(tracingEndpointString);
                otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });
        });

    // Build application
    app = builder.Build();
    app.Logger.LogInformation("Application has been build");
    // Register start/stop of the service
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => { app.Logger.LogInformation("Microservice Device Service has started"); });
    lifetime.ApplicationStopping.Register(() => { app.Logger.LogInformation("Microservice Device Service is stopping"); });
    app.Logger.LogInformation("Application starts initializing services");
    // Create and load device manager
    IDeviceManager deviceManager = app.Services.GetRequiredService<IDeviceManager>();

    using (IServiceScope scope = app.Services.CreateScope())
    {
        IDerRepository repository = scope.ServiceProvider.GetRequiredService<IDerRepository>();
        await deviceManager.LoadDerAsync(repository);
    }

    using (IServiceScope scope = app.Services.CreateScope())
    {
        // Load system configuration
        IRepository<DeviceServiceContext, SystemConfig> repository = scope.ServiceProvider.GetRequiredService<IRepository<DeviceServiceContext, SystemConfig>>();
        SystemConfig? config = repository.GetAsync(0, 1, (o) => o.Id).Result.FirstOrDefault();
        ArgumentNullException.ThrowIfNull(config, "System configuration contains no record");

        // Give the devices and device manager some time to connect.
        await Task.Delay(config.StartupDelayForDevices / 2);

        // Create and load Modbus polling service
        ModbusPollService modbusPollService = app.Services.GetRequiredService<ModbusPollService>();
        modbusPollService.Initialize(deviceManager.ModbusPollingEngines, config.PollingIntervalMilliseconds);

        // Give the devices and device manager some time to get some data before publishing
        await Task.Delay(config.StartupDelayForDevices / 2);

        // Create and load device publisher
        DevicePublisherService devicePublisherService = app.Services.GetRequiredService<DevicePublisherService>();
        ILogger<DevicePublisher> logger = app.Services.GetRequiredService<ILogger<DevicePublisher>>();
        IMessagePublisher messagePublisher = app.Services.GetRequiredService<IMessagePublisher>();
        DeviceMapper deviceMapper = new DeviceMapper();
        PublisherTopic topic = PublisherTopic.Create(config);
        DevicePublisher devicePublisher = new DevicePublisher(logger, deviceManager, deviceMapper, messagePublisher, topic, config.DeviceFactorCore, config.DeviceFactorDetail);
        devicePublisherService.Initialize(devicePublisher, config.MetricsIntervalMilliseconds);

        // Create and load metric publisher
        MetricsPublisherService metricsPublisherService = app.Services.GetRequiredService<MetricsPublisherService>();
        metricsPublisherService.Initialize(deviceManager.MetricsPublishers, config.MetricsIntervalMilliseconds);
    }
    app.Logger.LogInformation("Application finished initializing services");

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    string error = $"Device service startup failed: {ex.Message}";
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