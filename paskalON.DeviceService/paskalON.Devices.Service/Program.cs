// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Messaging;
using paskalON.Messaging.Redis;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);



// Build WebApi
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Build publishers
string redisConnectionString = "TODO:Get";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddSingleton<IMessagePublisher, RedisMessagePublisher>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
