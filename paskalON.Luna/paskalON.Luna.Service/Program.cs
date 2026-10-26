// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Client;
using paskalON.Devices.Client.Subscribers;
using paskalON.Luna.Application;
using paskalON.Luna.Domain.Configs;
using paskalON.Messaging;
using paskalON.Messaging.Redis;
using paskalON.OperatingModes.Client;
using paskalON.OperatingModes.Client.Subscribers;
using paskalON.PowerControls.Client.Subscribers;
using StackExchange.Redis;
using System.Xml.Serialization;

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

    // Create builder
    Console.WriteLine("Building service.....");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add WebApi's
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

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

    // Build application
    Console.WriteLine("Building application.....");
    app = builder.Build();
    app.Logger.LogInformation("Application built.....");
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => { app.Logger.LogInformation("Microservice Luna has started....."); });
    lifetime.ApplicationStopped.Register(() => { app.Logger.LogInformation("Microservice Luna has stopping....."); });
    app.Logger.LogInformation("Application starts initializing services.....");

    // Load system configuration
    SystemConfig config = LoadSystemConfig("SystemConfig.xml");
    SubscriberTopic topics = GetSubscriberTopic(config);
    IDeviceClient deviceClient = app.Services.GetRequiredService<IDeviceClient>();
    await deviceClient.Initialize(topics);

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
    app.Logger.LogInformation("Application about to run.....");
    app.Run();
}
catch (Exception ex)
{
    string error = $"Luna service startup failed: {ex.Message}";
    Console.WriteLine(error);
    Console.WriteLine(ex.StackTrace);

    if (app != null)
    {
        app.Logger.LogError(ex, "Service failed to start");
        app.Logger.LogError(ex.StackTrace);
    }

    throw;
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