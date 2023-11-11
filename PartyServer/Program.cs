using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using PartyModels;
using PartyServer;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var logger = new LoggerConfiguration()
    .WriteTo.Console(theme: SystemConsoleTheme.Literate)
    .CreateLogger();

var repo = new PartyAppRepository(logger,false);

app.MapGet("/GetMessages", () =>
{
    try
    {
        var messages = repo.GetMessages();
        logger.Information($"Returning {messages.Count} # of messages");
        return JsonSerializer.Serialize(messages);
    }
    catch (Exception e)
    {
        Console.Error.WriteLine("Error occurred during runtime: " + e.Message);
        return JsonSerializer.Serialize(new List<Message>());
    }
});

app.MapPost("/InsertMessage", async (Message message) =>
{
    try
    {
        repo.InsertMessage(message);
    }
    catch (Exception e)
    {
        logger.Error("Error occurred during runtime: " + e.Message);
    }
});

app.MapPost("/Upvote", async (Guid messageGuid) =>
{
    try
    {
        repo.UpvoteMessage(messageGuid);
    }
    catch (Exception e)
    {
        logger.Error("Error occurred during runtime: " + e.Message);
    }
});

app.MapPost("/Downvote", async (Guid messageGuid) =>
{
    try
    {
        repo.DownvoteMessage(messageGuid);
    }
    catch (Exception e)
    {
        logger.Error("Error occurred during runtime: " + e.Message);
    }
});

app.MapGet("/HealthCheck", () =>
{
    logger.Information("Health check called successfully");
    return "OK";
});

app.Run();