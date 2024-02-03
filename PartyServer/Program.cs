using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PartyModels;
using PartyServer;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    // options.SerializerOptions.WriteIndented = true;
    // options.SerializerOptions.IncludeFields = true;
});

var app = builder.Build();

var logger = new LoggerConfiguration()
    .WriteTo.Console(theme: SystemConsoleTheme.Literate)
    .CreateLogger();

var repo = new PartyAppRepository(logger);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Map("/", async () => Results.Text("hello"));

app.MapGet("/GetMessages", async () =>
{
    try
    {
        var messages = repo.GetMessages();
        logger.Information("GetMessages called successfully with {MessageCount} messages", messages.Count);
        return Results.Json(messages);
    }
    catch (Exception e)
    {
        var errorText = "Error occurred during runtime could not Get message: " + e.Message + " StackTrace:" + e.StackTrace;
        logger.Error(errorText);
        return Results.Problem(errorText);
    }
});


app.MapPost("/InsertMessage", ([FromBody]Message message) =>
{
    try
    { 
        repo.InsertMessage(message);
        return Results.Ok("insert success");
    }
    catch (Exception e)
    {
        var errorText = "Error occurred during runtime could not insert message: " + e.Message + " StackTrace:" +
                        e.StackTrace;
        logger.Error(errorText);
        return Results.Problem(errorText); }
});

app.MapPost("/Upvote", ([FromBody]Guid messageGuid) =>
{
    try
    {
        repo.UpvoteMessage(messageGuid);
        return Task.FromResult(Results.Ok("success"));
    }
    catch (Exception e)
    {
        var errorText = $"Error occurred during runtime could not upvote message: {e.Message} StackTrace:{e.StackTrace}";
        logger.Error(errorText);
        return Task.FromResult(Results.Problem(errorText));
    }
});

app.MapPost("/Downvote", ([FromBody]Guid messageGuid) =>
{
    try
    {
        repo.DownvoteMessage(messageGuid);
        return Task.FromResult(Results.Ok("success"));
    }
    catch (Exception e)
    {
        var errorText = "Error occurred during runtime: " + e.Message + " StackTrace:" + e.StackTrace;
        logger.Error(errorText);
        throw;
    }
});

app.MapGet("/HealthCheck", () =>
{
    logger.Information("Health check called successfully");
    return Results.Ok("success");
});

app.MapDelete("DeleteAllMessages", () =>
{
    try
    {
        var rowsDeleted = repo.DeleteAllMessages();
        return Results.Ok("deleted all messages successfully. Rows deleted:" + rowsDeleted);
    }
    catch (Exception e)
    {
        var errorText = "Error occurred during runtime could not delete all messages: " + e.Message + " StackTrace:" +
                        e.StackTrace;
        logger.Error(errorText);
        return Results.Problem(errorText);
    }
});

app.MapDelete("DeleteMessage", ([FromBody]Message message) =>
{
    try
    {
        repo.DeleteMessage(message);
        return Results.Ok("success");
    }
    catch (Exception e)
    {
        var errorText = "Error occurred during runtime could not delete message: " + e.Message + " StackTrace:" +
                        e.StackTrace;
        logger.Error(errorText);
        return Results.Problem(errorText);
    }
});

app.Run();