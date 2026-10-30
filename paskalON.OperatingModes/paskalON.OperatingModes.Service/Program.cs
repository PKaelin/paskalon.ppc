// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using paskalON.Devices.Client;
using paskalON.Devices.Client.Subscribers;
using paskalON.Hosting;
using paskalON.Infrastructure.Repositories;
using paskalON.Messaging;
using paskalON.Messaging.Health;
using paskalON.Messaging.Heartbeats;
using paskalON.Messaging.Redis;
using paskalON.Messaging.Sequences;
using paskalON.OperatingModes.Application;
using paskalON.OperatingModes.Application.Dependencies;
using paskalON.OperatingModes.Application.Factories;
using paskalON.OperatingModes.Application.Publishers;
using paskalON.OperatingModes.Application.Stacks;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Infrastructure.Storage;
using paskalON.OperatingModes.Infrastructure.Storage.Repositories;
using paskalON.OperatingModes.Service.Publishers;
using paskalON.OperatingModes.Service.Workers;
using paskalON.Telemetry;
using paskalON.Telemetry.Factories;
using StackExchange.Redis;

// Name of this service in the heartbeat and the source instance of the published messages
const string serviceName = "OperatingMode";
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

    string startWorkerString = Environment.GetEnvironmentVariable("START_WORKERS") ?? "true";
    if (bool.TryParse(startWorkerString, out var startWorker) == false)
    {
        throw new ApplicationException("START_WORKERS is not configured as a boolean");
    }

    // Get startup retry options for dependencies that are not yet available
    StartupRetryOptions startupRetryOptions = StartupRetryOptions.FromEnvironment();

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
    // Do not abort when Redis is not reachable at startup, the multiplexer keeps retrying to connect in the background.
    ConfigurationOptions msgOptions = ConfigurationOptions.Parse(msgConnectionString);
    msgOptions.AbortOnConnectFail = false;
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(msgOptions));
    builder.Services.AddSingleton<IMessagePublisher, RedisMessagePublisher>();
    builder.Services.AddSingleton<IMetricsPublisherFactory, MetricsPublisherFactory>();
    builder.Services.AddTransient<IMetricsPublisher, MetricsPublisher>();
    builder.Services.AddSingleton<MetricsPublisherService>();
    // Long living HTTP client for the device service API
    // TODO: Refactor to multiple
    Uri dsBaseAddress = new Uri($"{dsConnectionStrings[0].TrimEnd('/')}/{dsEndpoint.Trim('/')}/");
    builder.Services.AddKeyedSingleton(nameof(DeviceServer), (_, _) => new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) })
    {
        BaseAddress = dsBaseAddress,
        Timeout = TimeSpan.FromSeconds(10)
    });
    builder.Services.AddSingleton<IDeviceServer>(sp => new DeviceServer(sp.GetRequiredKeyedService<HttpClient>(nameof(DeviceServer))));
    builder.Services.AddSingleton<IMessageSubscriber, RedisMessageSubscriber>();
    builder.Services.AddSingleton<IDeviceClient, DeviceClient>();
    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddSingleton<IOperatingModeMapFactory, OperatingModeMapFactory>();
    builder.Services.AddSingleton<IOperatingModeFactory, OperatingModeFactory>();
    builder.Services.AddSingleton<OperatingModeService>();
    // Dependency monitoring, the operating mode control holds the setpoints while a dependency is lost
    builder.Services.AddSingleton<IDependencyHealthMonitor, DependencyHealthMonitor>();
    builder.Services.AddSingleton<HeartbeatService>();
    builder.Services.AddHostedService<HeartbeatService>(provider => provider.GetRequiredService<HeartbeatService>());

    builder.Services.AddSingleton<IOperatingModeManager, OperatingModeManager>();
    builder.Services.AddSingleton<IOperatingModeStackDtoFactory, OperatingModeStackDtoFactory>();

    if (startPublisher == true)
    {
        builder.Services.AddHostedService<MetricsPublisherService>(provider => provider.GetRequiredService<MetricsPublisherService>());
    }

    if (startWorker == true)
    {
        builder.Services.AddHostedService<OperatingModeService>(provider => provider.GetRequiredService<OperatingModeService>());
    }

    // Configure OpenTelemetry logging, metrics, & tracing with auto-start using the
    // AddOpenTelemetry extension from OpenTelemetry.Extensions.Hosting.
    builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r
        .Clear()
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
            builder.AddMeter("OperatingMode");

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
            builder.AddSource("PPC.OperatingModes");
            builder.AddOtlpExporter((otlpOptions) =>
            {
                otlpOptions.Endpoint = new Uri(tracingEndpointString);
                otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });
        });
    // TODO: Only publish telemetry when operating mode is enabled!

    Console.WriteLine("Building application.....");
    app = builder.Build();
    Console.WriteLine("Application built.....");
    // Register start/stop of the service
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => { app.Logger.LogInformation("Microservice Operating Mode has started."); });
    lifetime.ApplicationStopping.Register(() => { app.Logger.LogInformation("Microservice Operating Mode is stopping."); });
    app.Logger.LogInformation("Application starts initializing services.....");
    StartupRetry startupRetry = new StartupRetry(app.Logger, TimeProvider.System, startupRetryOptions);
    // Create and load device manager
    IOperatingModeManager manager = app.Services.GetRequiredService<IOperatingModeManager>();

    using (IServiceScope scope = app.Services.CreateScope())
    {
        // Retry while the database is not reachable
        IRepository<OperatingModeContext, SystemConfig> systemConfigRepository = scope.ServiceProvider.GetRequiredService<IRepository<OperatingModeContext, SystemConfig>>();
        SystemConfig config = await startupRetry.ExecuteAsync("Load system configuration",
            async () => (await systemConfigRepository.GetAsync(0, 1, o => o.Id)).Single(), DatabaseTransientExceptionFilter.IsTransient);
        // Register the dependencies before subscribing, every dependency is lost until it is seen the first time
        IDependencyHealthMonitor dependencyHealthMonitor = app.Services.GetRequiredService<IDependencyHealthMonitor>();
        IConnectionMultiplexer connectionMultiplexer = app.Services.GetRequiredService<IConnectionMultiplexer>();
        dependencyHealthMonitor.RegisterCondition(DependencyNames.Redis, () => connectionMultiplexer.IsConnected,
            TimeSpan.FromMilliseconds(config.RedisLossTimeoutMilliseconds));
        dependencyHealthMonitor.RegisterSignal(DependencyNames.DeviceService, TimeSpan.FromMilliseconds(config.DeviceServiceLossTimeoutMilliseconds));
        RegisterPowerControlHeartbeat(app, config, dependencyHealthMonitor);
        // Load and initialize device client, retry while the device service is not reachable. Received device data signals the device service.
        SubscriberTopic topics = GetSubscriberTopic(config);
        IDeviceClient deviceClient = app.Services.GetRequiredService<IDeviceClient>();
        deviceClient.CoreDataReceived += (_, _) => dependencyHealthMonitor.Signal(DependencyNames.DeviceService);
        await startupRetry.ExecuteAsync("Initialize device client", () => deviceClient.Initialize(topics), HttpTransientExceptionFilter.IsTransient);
        // Create setpoints publisher with the configured topics
        PublisherTopic publisherTopic = PublisherTopic.Create(config);
        ILogger<SetpointsPublisher> setpointsPublisherLogger = app.Services.GetRequiredService<ILogger<SetpointsPublisher>>();
        IMessagePublisher messagePublisher = app.Services.GetRequiredService<IMessagePublisher>();
        // A new source instance per process start, so that the receivers do not drop the restarted sequences
        string sourceInstance = MessageSequencer.CreateSourceInstance(serviceName);
        InitializeHeartbeat(app, config, messagePublisher, sourceInstance);
        SetpointsPublisher setpointsPublisher = new SetpointsPublisher(setpointsPublisherLogger, messagePublisher, publisherTopic,
            new MessageSequencer(sourceInstance, TimeProvider.System));
        // Load and initialize manager, retry while the database is not reachable
        IOperatingModeRepository repository = scope.ServiceProvider.GetRequiredService<IOperatingModeRepository>();
        await startupRetry.ExecuteAsync("Initialize operating mode manager", () => manager.Initialize(repository, config, setpointsPublisher), DatabaseTransientExceptionFilter.IsTransient);

        if (startPublisher == true)
        {
            MetricsPublisherService metricsPublisherService = app.Services.GetRequiredService<MetricsPublisherService>();
            metricsPublisherService.Initialize(manager.MetricsPublishers, config.MetricsIntervalMilliseconds, config.StartupDelay);
        }

        if (startWorker == true)
        {
            OperatingModeService operatingModeService = app.Services.GetRequiredService<OperatingModeService>();
            operatingModeService.Initialize(config.OperatingModeCalculationIntervalMilliseconds);
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


/// <summary>
/// Register the power control as dependency and subscribe to its heartbeat.
/// </summary>
/// <remarks>
/// Without a configured heartbeat topic the power control is not monitored and a loss of the power control is not detected.
/// </remarks>
static void RegisterPowerControlHeartbeat(WebApplication app, SystemConfig config, IDependencyHealthMonitor dependencyHealthMonitor)
{
    if (string.IsNullOrWhiteSpace(config.SubscriberTopicPowerControlHeartbeat))
    {
        app.Logger.LogWarning("No power control heartbeat topic is configured, a loss of the power control is not detected.");

        return;
    }

    dependencyHealthMonitor.RegisterSignal(DependencyNames.PowerControl, TimeSpan.FromMilliseconds(config.PowerControlLossTimeoutMilliseconds));
    HeartbeatSubscriber heartbeatSubscriber = new HeartbeatSubscriber(app.Services.GetRequiredService<ILogger<HeartbeatSubscriber>>(),
        app.Services.GetRequiredService<IMessageSubscriber>(), dependencyHealthMonitor, config.SubscriberTopicPowerControlHeartbeat, DependencyNames.PowerControl);
    heartbeatSubscriber.Subscribe();
}


/// <summary>
/// Initialize the heartbeat service that publishes the heartbeat of this operating mode service.
/// </summary>
static void InitializeHeartbeat(WebApplication app, SystemConfig config, IMessagePublisher messagePublisher, string sourceInstance)
{
    if (string.IsNullOrWhiteSpace(config.PublisherTopicHeartbeat))
    {
        app.Logger.LogWarning("No heartbeat topic is configured, the heartbeat of the operating mode is not published.");

        return;
    }

    MessageSequencer sequencer = new MessageSequencer(sourceInstance, TimeProvider.System);
    HeartbeatPublisher heartbeatPublisher = new HeartbeatPublisher(app.Services.GetRequiredService<ILogger<HeartbeatPublisher>>(), messagePublisher, sequencer,
        serviceName, config.PublisherTopicHeartbeat, TimeSpan.FromMilliseconds(config.DependencyHeartbeatIntervalMilliseconds), TimeProvider.System);
    app.Services.GetRequiredService<HeartbeatService>().Initialize(heartbeatPublisher);
}


/// <summary>
/// Get subscriber topic configuration object.
/// </summary>
static SubscriberTopic GetSubscriberTopic(SystemConfig config)
{
    SubscriberTopic topic = new SubscriberTopic();

    if (config.SubscriberTopicPcsCore != null && config.SubscriberTopicPcsDetail != null)
    {
        topic.PowerConversionSystemTopic = new SubscriberTopicEntry(config.SubscriberTopicPcsCore, config.SubscriberTopicPcsDetail);
    }

    if (config.SubscriberTopicBatteryBankCore != null && config.SubscriberTopicBatteryBankDetail != null)
    {
        topic.BatteryBankTopic = new SubscriberTopicEntry(config.SubscriberTopicBatteryBankCore, config.SubscriberTopicBatteryBankDetail);
    }

    if (config.SubscriberTopicSolarPanelCore != null && config.SubscriberTopicSolarPanelDetail != null)
    {
        topic.SolarPanelTopic = new SubscriberTopicEntry(config.SubscriberTopicSolarPanelCore, config.SubscriberTopicSolarPanelDetail);
    }

    if (config.SubscriberTopicExternalPowerMeterCore != null && config.SubscriberTopicExternalPowerMeterDetail != null)
    {
        topic.ExternalPowerMeterTopic = new SubscriberTopicEntry(config.SubscriberTopicExternalPowerMeterCore, config.SubscriberTopicExternalPowerMeterDetail);
    }

    if (config.SubscriberTopicAuxiliaryPowerMeterCore != null && config.SubscriberTopicAuxiliaryPowerMeterDetail != null)
    {
        topic.AuxiliaryPowerMeterTopic = new SubscriberTopicEntry(config.SubscriberTopicAuxiliaryPowerMeterCore, config.SubscriberTopicAuxiliaryPowerMeterDetail);
    }

    if (config.SubscriberTopicSystemPowerMeterCore != null && config.SubscriberTopicSystemPowerMeterDetail != null)
    {
        topic.SystemPowerMeterTopic = new SubscriberTopicEntry(config.SubscriberTopicSystemPowerMeterCore, config.SubscriberTopicSystemPowerMeterDetail);
    }

    if (config.SubscriberTopicCircuitPowerMeterCore != null && config.SubscriberTopicCircuitPowerMeterDetail != null)
    {
        topic.CircuitPowerMeterTopic = new SubscriberTopicEntry(config.SubscriberTopicCircuitPowerMeterCore, config.SubscriberTopicCircuitPowerMeterDetail);
    }
    return topic;
}
