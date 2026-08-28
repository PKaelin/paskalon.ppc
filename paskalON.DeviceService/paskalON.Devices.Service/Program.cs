// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.Devices.Application;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Infrastructure.Storage;
using paskalON.Devices.Infrastructure.Storage.Repositories;
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

    // Add WebApis
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // Add databases
    builder.Services.AddDbContext<DeviceServiceContext>(options => options.UseNpgsql(dbConnectionString));
    builder.Services.AddScoped<IDerRepository, DerRepository>();

    // Add communications
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(msgConnectionString));
    builder.Services.AddSingleton<IMessagePublisher, RedisMessagePublisher>();
    builder.Services.AddSingleton<IModbusDeviceFactory, ModbusDeviceFactory>();
    builder.Services.AddSingleton<IC37DeviceFactory, C37DeviceFactory>();
    builder.Services.AddSingleton<IMetricsPublisherFactory, MetricsPublisherFactory>();
    builder.Services.AddTransient<IMetricsPublisher, MetricsPublisher>();

    // Build application
    var app = builder.Build();

    // Create and load device manager
    using (IServiceScope scope = app.Services.CreateScope())
    {
        ILogger<DeviceManager> logger = app.Services.GetRequiredService<ILogger<DeviceManager>>();
        IDerRepository repository = scope.ServiceProvider.GetRequiredService<IDerRepository>();
        IModbusDeviceFactory deviceFactoryModbus = app.Services.GetRequiredService<IModbusDeviceFactory>();
        IC37DeviceFactory deviceFactoryC37 = app.Services.GetRequiredService<IC37DeviceFactory>();
        IMetricsPublisherFactory publisherFactory = app.Services.GetRequiredService<IMetricsPublisherFactory>();
        DeviceManager deviceManager = new DeviceManager(logger, repository, app.Services, publisherFactory, deviceFactoryModbus, deviceFactoryC37);
        Console.WriteLine("Loading DERs.....");
        await deviceManager.LoadDerAsync();
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