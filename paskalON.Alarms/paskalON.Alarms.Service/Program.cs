// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Application.Outboxes;
using paskalON.Alarms.Application.Stores;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Infrastructure.Storage;
using paskalON.Alarms.Infrastructure.Storage.Repositories;
using paskalON.Alarms.Service.Publishers;
using paskalON.Hosting;
using paskalON.Infrastructure.Repositories;
using paskalON.Messaging.RabbitMq;

// Name of this service in the heartbeat and the source instance of the published messages
const string serviceName = "Alarm";
// Name of this service in the RabbitMQ connections
const string clientName = "paskalON.Alarms";
WebApplication? app = null;
// Loaded after the application is built, read when the singletons are created on first use
SystemConfig? systemConfig = null;
Console.WriteLine("Starting service.....");

try
{
    Console.WriteLine("Getting environments.....");
    // Get database connection string
    string? dbConnectionStringFile = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_FILE");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dbConnectionStringFile, "Cannot find the database secret file. DATABASE_CONNECTION_FILE");
    string dbConnectionString = (await File.ReadAllTextAsync(dbConnectionStringFile)).Trim();
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(dbConnectionString, "Cannot find the database connection string definition");

    // Get RabbitMQ connection URI, e.g. amqp://user:password@paskalon.rabbitmq:5672/
    string? rabbitMqConnectionFile = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_FILE");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(rabbitMqConnectionFile, "Cannot find the RabbitMQ secret file. RABBITMQ_CONNECTION_FILE");
    string rabbitMqConnectionString = (await File.ReadAllTextAsync(rabbitMqConnectionFile)).Trim();
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(rabbitMqConnectionString, "Cannot find the RabbitMQ connection string definition");
    Uri rabbitMqConnectionUri = new Uri(rabbitMqConnectionString);

    // Get Logging, Metrics, Tracing endpoint strings
    string? logEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_LOGGING_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(logEndpointString, "Cannot find the logging endpoint. TELEMETRY_LOGGING_ENDPOINT");
    string? metricsEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_METRICS_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(metricsEndpointString, "Cannot find the metrics endpoint. TELEMETRY_METRICS_ENDPOINT");
    string? tracingEndpointString = Environment.GetEnvironmentVariable("TELEMETRY_TRACING_ENDPOINT");
    ArgumentOutOfRangeException.ThrowIfNullOrEmpty(tracingEndpointString, "Cannot find the tracing endpoint. TELEMETRY_TRACING_ENDPOINT");

    // Get start options
    string startPublisherString = Environment.GetEnvironmentVariable("START_PUBLISHERS") ?? "true";

    if (bool.TryParse(startPublisherString, out bool startPublisher) is false)
    {
        throw new ApplicationException("START_PUBLISHERS is not configured as a boolean");
    }

    // Get startup retry options for dependencies that are not yet available
    StartupRetryOptions startupRetryOptions = StartupRetryOptions.FromEnvironment();

    Console.WriteLine("Create builder service.....");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add WebApi's
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // Add database
    builder.Services.AddDbContext<AlarmContext>(options => options.UseNpgsql(dbConnectionString));
    builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
    builder.Services.AddScoped<IPpcAlarmConfigRepository, PpcAlarmConfigRepository>();
    builder.Services.AddScoped<IPpcAlarmRepository, PpcAlarmRepository>();
    builder.Services.AddScoped<IAlarmActionOutboxRepository, AlarmActionOutboxRepository>();

    // Add alarm processing
    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddSingleton<IPpcAlarmValueStore, PpcAlarmValueStore>();
    builder.Services.AddSingleton<IPpcAlarmEvaluator, PpcAlarmEvaluator>();
    // Signals the saved alarm actions from the scoped handlers to the outbox publisher service, so that they are sent immediately
    builder.Services.AddSingleton<IAlarmActionOutboxSignal, AlarmActionOutboxSignal>();
    builder.Services.AddScoped<IPpcAlarmHandler, PpcAlarmHandler>();

    // Add alarm actions: the publisher keeps the RabbitMQ connection (singleton), the sender uses the repository (scoped).
    // The publisher is created on the first send, after the system configuration is loaded.
    builder.Services.AddSingleton<IRabbitMqPublisher>(provider => new RabbitMqPublisher(provider.GetRequiredService<ILogger<RabbitMqPublisher>>(),
        rabbitMqConnectionUri, GetAlarmActionExchange(systemConfig), clientName));
    builder.Services.AddScoped<IAlarmActionOutboxSender>(provider => new AlarmActionOutboxSender(
        provider.GetRequiredService<ILogger<AlarmActionOutboxSender>>(), provider.GetRequiredService<IAlarmActionOutboxRepository>(),
        provider.GetRequiredService<IRabbitMqPublisher>(), provider.GetRequiredService<TimeProvider>()));
    builder.Services.AddSingleton<AlarmActionOutboxPublisherService>();

    if (startPublisher == true)
    {
        builder.Services.AddHostedService<AlarmActionOutboxPublisherService>(provider => provider.GetRequiredService<AlarmActionOutboxPublisherService>());
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
            builder.AddMeter(serviceName);

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
            builder.AddSource("PPC.Alarms");
            builder.AddOtlpExporter((otlpOptions) =>
            {
                otlpOptions.Endpoint = new Uri(tracingEndpointString);
                otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });
        });

    Console.WriteLine("Building application.....");
    app = builder.Build();
    Console.WriteLine("Application built.....");
    // Register start/stop of the service
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => { app.Logger.LogInformation("Microservice Alarm has started."); });
    lifetime.ApplicationStopping.Register(() => { app.Logger.LogInformation("Microservice Alarm is stopping."); });
    app.Logger.LogInformation("Application starts initializing services.....");
    StartupRetry startupRetry = new StartupRetry(app.Logger, TimeProvider.System, startupRetryOptions);

    using (IServiceScope scope = app.Services.CreateScope())
    {
        // Load system configuration, retry while the database is not reachable
        IRepository<AlarmContext, SystemConfig> systemConfigRepository = scope.ServiceProvider.GetRequiredService<IRepository<AlarmContext, SystemConfig>>();
        systemConfig = await startupRetry.ExecuteAsync("Load system configuration",
            async () => (await systemConfigRepository.GetAsync(0, 1, o => o.Id)).Single(), DatabaseTransientExceptionFilter.IsTransient);
        // Fail the startup instead of the first send when the alarm action exchange is missing
        string alarmActionExchange = GetAlarmActionExchange(systemConfig);
        app.Logger.LogInformation("Alarm actions are published to exchange {ExchangeName}.", alarmActionExchange);

        if (startPublisher == true)
        {
            AlarmActionOutboxPublisherService outboxPublisherService = app.Services.GetRequiredService<AlarmActionOutboxPublisherService>();
            outboxPublisherService.Initialize(systemConfig.AlarmActionSendIntervalMilliseconds, systemConfig.AlarmActionRetryIntervalMilliseconds);
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
/// Get the configured exchange of the alarm actions.
/// </summary>
/// <exception cref="InvalidOperationException">Thrown when the system configuration is not loaded or has no alarm action exchange.</exception>
static string GetAlarmActionExchange(SystemConfig? config)
{
    if (config is null)
    {
        throw new InvalidOperationException("The system configuration has not been loaded.");
    }

    if (string.IsNullOrWhiteSpace(config.PublisherExchangeAlarmActions))
    {
        throw new InvalidOperationException($"No alarm action exchange is configured. {nameof(SystemConfig)}.{nameof(SystemConfig.PublisherExchangeAlarmActions)}");
    }

    return config.PublisherExchangeAlarmActions;
}
