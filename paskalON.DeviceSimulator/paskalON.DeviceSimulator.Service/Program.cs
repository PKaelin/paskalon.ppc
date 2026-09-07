// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------

WebApplication? app = null;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    app = builder.Build();
    app.Logger.LogInformation("Application has been built");
    IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStarted.Register(() => app.Logger.LogInformation("Microservice Device Simulator has started"));
    lifetime.ApplicationStopping.Register(() => app.Logger.LogInformation("Microservice Device Simulator is stopping"));
    app.Logger.LogInformation("Application starts initializing services");


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