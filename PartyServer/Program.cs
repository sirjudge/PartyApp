using System.Text.Json;
using PartyApp;
using PartyModels;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/GetMessages", () =>
{
    var repo = new PartyAppRepository();
    var messages = repo.GetMessages();
    return JsonSerializer.Serialize(messages);
});

app.MapPost("/InsertMessage", async (Message message) =>
{
    var repo = new PartyAppRepository();
    repo.InsertMessage(message);
    return await Task.FromResult(true);
});

app.MapPost("/Upvote", async (Guid messageGuid) =>
{
    throw new NotImplementedException();
});

app.MapPost("/Downvote", async (Guid messageGuid) =>
{
    throw new NotImplementedException();
});

app.MapGet("/HealthCheck", () => "Healthy");

app.Run();