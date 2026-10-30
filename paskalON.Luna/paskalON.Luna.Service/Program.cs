// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.SignalR;
using paskalON.Devices.Client;
using paskalON.Devices.Client.Subscribers;
using paskalON.Hosting;
using paskalON.Luna.Application;
using paskalON.Luna.Application.Dependencies;
using paskalON.Luna.Domain.Configs;
using paskalON.Luna.Dto.Systems;
using paskalON.Luna.Service.Publishers;
using paskalON.Luna.Service.WebApi.Hubs;
using paskalON.Messaging;
using paskalON.Messaging.Health;
using paskalON.Messaging.Heartbeats;
using paskalON.Messaging.Redis;
using paskalON.OperatingModes.Client;
using paskalON.OperatingModes.Client.Subscribers;
using paskalON.PowerControls.Client.Subscribers;
using StackExchange.Redis;
using System.Xml.Serialization;

const string serviceName = "Luna";
WebApplication? app = null;
Console.WriteLine("Starting service.....");

try
{
    Console.WriteLine("Getting environments.....");
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
    // Get device service DER endpoint
    string? dsEndpoint = Environment.GetEnvironmentVariable("DEVICE_SERVICE_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dsEndpoint, "Cannot find the device service endpoint. DEVICE_SERVICE_ENDPOINT");

    // Get operating mode service connection string
    string? omConnectionStringFile = Environment.GetEnvironmentVariable("OPERATINGMODE_CONNECTION_FILE");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(omConnectionStringFile, "Cannot find the operating mode service secret file. OPERATINGMODE_CONNECTION_FILE");
    string omConnectionString = (await File.ReadAllTextAsync(omConnectionStringFile)).Trim();
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(omConnectionString, "Cannot find the operating mode service connection string definition");
    // Get operating mode service endpoint
    string? omEndpoint = Environment.GetEnvironmentVariable("OPERATINGMODE_SERVICE_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(omEndpoint, "Cannot find the operating mode service endpoint. OPERATINGMODE_SERVICE_ENDPOINT");

    // Get Logging, Metrics, Tracing endpoint strings
    string? logEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_LOGGING_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(logEndpointString, "Cannot find the logging endpoint. TELEMETRY_LOGGING_ENDPOINT");
    string? metricsEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_METRICS_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(metricsEndpointString, "Cannot find the metrics endpoint. TELEMETRY_METRICS_ENDPOINT");
    string? tracingEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_TRACING_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(tracingEndpointString, "Cannot find the tracing endpoint. TELEMETRY_TRACING_ENDPOINT");

    // Get startup retry options for dependencies that are not yet available
    StartupRetryOptions startupRetryOptions = StartupRetryOptions.FromEnvironment();

    // Create builder
    Console.WriteLine("Building service.....");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Load system configuration
    SystemConfig config = LoadSystemConfig("SystemConfig.xml");
    string shutdownOperatingModeName = config.ShutdownOperatingModeName ?? string.Empty;
    ArgumentException.ThrowIfNullOrWhiteSpace(shutdownOperatingModeName, "Cannot find the shutdown operating mode name. ShutdownOperatingModeName");
    SystemNameplateDto nameplate = GetSystemNameplate(config);

    // Add WebApi's
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddSignalR();
    builder.Services.AddSingleton(nameplate);

    // Add communications
    // Do not abort when Redis is not reachable at startup, the multiplexer keeps retrying to connect in the background.
    ConfigurationOptions msgOptions = ConfigurationOptions.Parse(msgConnectionString);
    msgOptions.AbortOnConnectFail = false;
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(msgOptions));
    builder.Services.AddSingleton<IMessageSubscriber, RedisMessageSubscriber>();

    // Long living HTTP client for the device service API
    Uri dsBaseAddress = new Uri($"{dsConnectionString.TrimEnd('/')}/{dsEndpoint.Trim('/')}/");
    builder.Services.AddKeyedSingleton(nameof(DeviceServer), (_, _) => new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) })
    {
        BaseAddress = dsBaseAddress,
        Timeout = TimeSpan.FromSeconds(10)
    });
    builder.Services.AddSingleton<IDeviceServer>(sp => new DeviceServer(sp.GetRequiredKeyedService<HttpClient>(nameof(DeviceServer))));
    builder.Services.AddSingleton<IDeviceClient, DeviceClient>();

    // Long living HTTP client for the operating mode service API
    Uri omBaseAddress = new Uri($"{omConnectionString.TrimEnd('/')}/{omEndpoint.Trim('/')}/");
    builder.Services.AddKeyedSingleton(nameof(OperatingModeServer), (_, _) => new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) })
    {
        BaseAddress = omBaseAddress,
        Timeout = TimeSpan.FromSeconds(10)
    });
    builder.Services.AddSingleton<IOperatingModeServer>(sp => new OperatingModeServer(sp.GetRequiredKeyedService<HttpClient>(nameof(OperatingModeServer))));

    // Add managers
    builder.Services.AddSingleton<ILunaDeviceManager, LunaDeviceManager>();
    // The managers are also the handlers of the received messages, so both interfaces resolve to the same instance
    builder.Services.AddSingleton<LunaPowerControlManager>();
    builder.Services.AddSingleton<ILunaPowerControlManager>(provider => provider.GetRequiredService<LunaPowerControlManager>());
    builder.Services.AddSingleton<IPowerTargetHandler>(provider => provider.GetRequiredService<LunaPowerControlManager>());
    builder.Services.AddSingleton<IPowerTargetSubscriber, PowerTargetSubscriber>();
    builder.Services.AddHostedService<PowerTargetSubscriberService>();

    builder.Services.AddSingleton<LunaOperatingModeManager>();
    builder.Services.AddSingleton<ILunaOperatingModeManager>(provider => provider.GetRequiredService<LunaOperatingModeManager>());
    builder.Services.AddSingleton<IPowerSetpointHandler>(provider => provider.GetRequiredService<LunaOperatingModeManager>());
    builder.Services.AddSingleton<IPowerSetpointSubscriber, PowerSetpointSubscriber>();
    builder.Services.AddHostedService<PowerSetpointSubscriberService>();
    // Dependency monitoring and pushing the availability of the dependencies to the UI clients connected to the dependency hub
    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddSingleton<IDependencyHealthMonitor, DependencyHealthMonitor>();
    builder.Services.AddSingleton<ILunaDependencyManager, LunaDependencyManager>();
    builder.Services.AddHostedService(sp => new DependencyStatusPublisher(
        sp.GetRequiredService<ILogger<DependencyStatusPublisher>>(),
        sp.GetRequiredService<ILunaDependencyManager>(),
        sp.GetRequiredService<IHubContext<DependencyHub, IDependencyClient>>(),
        GetDependencyStatusInterval(config)));

    builder.Services.AddSingleton<ILunaSystemManager>(sp => new LunaSystemManager(
        sp.GetRequiredService<ILogger<LunaSystemManager>>(),
        sp.GetRequiredService<IOperatingModeServer>(),
        sp.GetRequiredService<IDeviceServer>(),
        shutdownOperatingModeName));

    // Push the system meter summary to the UI clients connected to the system hub
    builder.Services.AddSingleton<HubConnectionCounter>();
    builder.Services.AddHostedService(sp => new SystemMeterSummaryPublisher(
        sp.GetRequiredService<ILogger<SystemMeterSummaryPublisher>>(),
        sp.GetRequiredService<ILunaDeviceManager>(),
        sp.GetRequiredService<IHubContext<SystemHub, ISystemClient>>(),
        sp.GetRequiredService<HubConnectionCounter>(),
        TimeSpan.FromSeconds(2)));

    // Push the values of the device shown in the system tab to the UI client that subscribed to it on the device hub
    builder.Services.AddSingleton<DeviceSubscriptions>();
    builder.Services.AddHostedService(sp => new DeviceSubscriptionPublisher(
        sp.GetRequiredService<ILogger<DeviceSubscriptionPublisher>>(),
        sp.GetRequiredService<ILunaDeviceManager>(),
        sp.GetRequiredService<IHubContext<DeviceHub, IDeviceHubClient>>(),
        sp.GetRequiredService<DeviceSubscriptions>(),
        TimeSpan.FromSeconds(2)));

    // Build application
    Console.WriteLine("Building application.....");
    app = builder.Build();
    app.Logger.LogInformation("Application built.....");
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => { app.Logger.LogInformation("Microservice Luna has started....."); });
    lifetime.ApplicationStopped.Register(() => { app.Logger.LogInformation("Microservice Luna has stopping....."); });
    app.Logger.LogInformation("Application starts initializing services.....");
    StartupRetry startupRetry = new StartupRetry(app.Logger, TimeProvider.System, startupRetryOptions);

    // Register the dependencies before subscribing, every dependency is lost until it is seen the first time
    IDependencyHealthMonitor dependencyHealthMonitor = app.Services.GetRequiredService<IDependencyHealthMonitor>();
    IConnectionMultiplexer connectionMultiplexer = app.Services.GetRequiredService<IConnectionMultiplexer>();
    dependencyHealthMonitor.RegisterCondition(DependencyNames.Redis, () => connectionMultiplexer.IsConnected,
        TimeSpan.FromMilliseconds(config.RedisLossTimeoutMilliseconds));
    dependencyHealthMonitor.RegisterSignal(DependencyNames.DeviceService, TimeSpan.FromMilliseconds(config.DeviceServiceLossTimeoutMilliseconds));
    dependencyHealthMonitor.RegisterSignal(DependencyNames.PowerControl, TimeSpan.FromMilliseconds(config.PowerControlLossTimeoutMilliseconds));
    dependencyHealthMonitor.RegisterSignal(DependencyNames.OperatingMode, TimeSpan.FromMilliseconds(config.OperatingModeLossTimeoutMilliseconds));
    SubscribeHeartbeat(app, dependencyHealthMonitor, config.SubscriberTopicPowerControlHeartbeat, DependencyNames.PowerControl);
    SubscribeHeartbeat(app, dependencyHealthMonitor, config.SubscriberTopicOperatingModeHeartbeat, DependencyNames.OperatingMode);

    // Initialize the device client with the subscriber topics of the system configuration, retry while the device service is not reachable. Received device data signals the device service.
    SubscriberTopic topics = GetSubscriberTopic(config);
    IDeviceClient deviceClient = app.Services.GetRequiredService<IDeviceClient>();
    deviceClient.CoreDataReceived += (_, _) => dependencyHealthMonitor.Signal(DependencyNames.DeviceService);
    await startupRetry.ExecuteAsync("Initialize device client", () => deviceClient.Initialize(topics), HttpTransientExceptionFilter.IsTransient);

    // Subscribe to the power targets and setpoints, they are handled once the application starts running
    IPowerTargetSubscriber powerTargetSubscriber = app.Services.GetRequiredService<IPowerTargetSubscriber>();
    powerTargetSubscriber.Subscribe(config.SubscriberTopicPowerTarget);
    IPowerSetpointSubscriber powerSetpointSubscriber = app.Services.GetRequiredService<IPowerSetpointSubscriber>();
    powerSetpointSubscriber.Subscribe(config.SubscriberTopicCurrentSetpoint);
    powerSetpointSubscriber.SubscribeDerUnit(config.SubscriberTopicCurrentDerUnitSetpoint);

    app.Logger.LogInformation("Application finished initializing services.....");

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<SystemHub>(SystemHub.Route);
    app.MapHub<DeviceHub>(DeviceHub.Route);
    app.MapHub<DependencyHub>(DependencyHub.Route);
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
/// Subscribe to the heartbeat of a dependency.
/// </summary>
/// <remarks>
/// Without a configured heartbeat topic the dependency stays lost.
/// </remarks>
static void SubscribeHeartbeat(WebApplication app, IDependencyHealthMonitor dependencyHealthMonitor, string? topic, string dependencyName)
{
    if (string.IsNullOrWhiteSpace(topic))
    {
        app.Logger.LogWarning("No heartbeat topic is configured for {Dependency}, it is reported as lost.", dependencyName);

        return;
    }

    HeartbeatSubscriber heartbeatSubscriber = new HeartbeatSubscriber(app.Services.GetRequiredService<ILogger<HeartbeatSubscriber>>(),
        app.Services.GetRequiredService<IMessageSubscriber>(), dependencyHealthMonitor, topic, dependencyName);
    heartbeatSubscriber.Subscribe();
}


/// <summary>
/// Get the interval to publish the dependency status, the shortest loss timeout of all dependencies.
/// </summary>
static TimeSpan GetDependencyStatusInterval(SystemConfig config)
{
    int[] lossTimeoutsMilliseconds = new int[]
    {
        config.RedisLossTimeoutMilliseconds,
        config.DeviceServiceLossTimeoutMilliseconds,
        config.PowerControlLossTimeoutMilliseconds,
        config.OperatingModeLossTimeoutMilliseconds
    };

    return TimeSpan.FromMilliseconds(lossTimeoutsMilliseconds.Min());
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


/// <summary>
/// Get the system nameplate ratings of the system configuration.
/// </summary>
static SystemNameplateDto GetSystemNameplate(SystemConfig config)
{
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(config.MaximumActivePowerSystemNameplateWatts, config.MinimumActivePowerSystemNameplateWatts);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(config.MaximumReactivePowerSystemNameplateVars, config.MinimumReactivePowerSystemNameplateVars);

    return new SystemNameplateDto
    {
        MaximumActivePowerWatts = config.MaximumActivePowerSystemNameplateWatts,
        MinimumActivePowerWatts = config.MinimumActivePowerSystemNameplateWatts,
        MaximumReactivePowerVars = config.MaximumReactivePowerSystemNameplateVars,
        MinimumReactivePowerVars = config.MinimumReactivePowerSystemNameplateVars
    };
}


/// <summary>
/// Load system configuration from XML file.
/// </summary>
static SystemConfig LoadSystemConfig(string filePath)
{
    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException($"System configuration file not found: {filePath}");
    }

    XmlSerializer serializer = new XmlSerializer(typeof(SystemConfig));
    using FileStream fs = new FileStream(filePath, FileMode.Open);
    {
        SystemConfig? config = serializer.Deserialize(fs) as SystemConfig;

        if (config == null)
        {
            throw new InvalidOperationException($"Failed to deserialize system configuration from file: {filePath}");
        }

        return config;
    }
}