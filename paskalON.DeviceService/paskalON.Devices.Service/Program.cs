// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.Devices.Application;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Application.Publishers;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Infrastructure.Storage;
using paskalON.Devices.Infrastructure.Storage.Repositories;
using paskalON.Devices.Service.Publishers;
using paskalON.Messaging;
using paskalON.Messaging.Redis;
using paskalON.Telemetry;
using StackExchange.Redis;

try
{
    Console.WriteLine("Getting environments.....");
    // Get database connection string
    string? dbConnectionStringFile = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_FILE");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dbConnectionStringFile, "Cannot find the database secret file");
    string dbConnectionString = (await File.ReadAllTextAsync(dbConnectionStringFile)).Trim();
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dbConnectionString, "Cannot find the database connection string definition");

    // Get messaging connection string
    string? msgConnectionStringFile = Environment.GetEnvironmentVariable("MESSAGING_CONNECTION_FILE");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(msgConnectionStringFile, "Cannot find the messaging secret file");
    string msgConnectionString = (await File.ReadAllTextAsync(msgConnectionStringFile)).Trim();
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(msgConnectionString, "Cannot find the messaging connection string definition");

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
    builder.Services.AddSingleton<DevicePublisherService>();
    builder.Services.AddHostedService<DevicePublisherService>(provider => provider.GetRequiredService<DevicePublisherService>());
    builder.Services.AddSingleton<MetricsPublisherService>();
    builder.Services.AddHostedService<MetricsPublisherService>(provider => provider.GetRequiredService<MetricsPublisherService>());

    // Build application
    var app = builder.Build();

    // Create and load device manager
    DeviceManager deviceManager;
    using (IServiceScope scope = app.Services.CreateScope())
    {
        ILogger<DeviceManager> logger = app.Services.GetRequiredService<ILogger<DeviceManager>>();
        IDerRepository repository = scope.ServiceProvider.GetRequiredService<IDerRepository>();
        IModbusDeviceFactory deviceFactoryModbus = app.Services.GetRequiredService<IModbusDeviceFactory>();
        IC37DeviceFactory deviceFactoryC37 = app.Services.GetRequiredService<IC37DeviceFactory>();
        IMetricsPublisherFactory publisherFactory = app.Services.GetRequiredService<IMetricsPublisherFactory>();
        deviceManager = new DeviceManager(logger, repository, app.Services, publisherFactory, deviceFactoryModbus, deviceFactoryC37);
        Console.WriteLine("Loading DERs.....");
        await deviceManager.LoadDerAsync();
    }

    using (IServiceScope scope = app.Services.CreateScope())
    {
        // Load system configuration
        IRepository<DeviceServiceContext, SystemConfig> repository = scope.ServiceProvider.GetRequiredService<IRepository<DeviceServiceContext, SystemConfig>>();
        SystemConfig? config = repository.GetAsync(0, 1).Result.FirstOrDefault();
        ArgumentNullException.ThrowIfNull(config, "System configuration contains no record");

        // Give the devices and device manager some time to connect and get ready.
        await Task.Delay(config.StartupDelayForDevices);

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
    // Output to Dockers standard output. Use: docker logs [container]
    Console.Error.WriteLine($"Device service startup failed: {ex.Message}");
    Console.Error.WriteLine(ex.StackTrace);
    throw;
}