using Azure.AI.OpenAI;
using BasicOpenAIChatService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddSingleton(new OpenAIClient("..."));
builder.Services.AddSingleton<ChatManager>();

var app = builder.Build();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapGet("/", () => "Hello World!");

app.MapPost("/chat/sessions", (ChatManager chatManager) =>
{
    var session = chatManager.CreateSession();
    return Results.Created((string?)null, new { SessionId = session.Id });
});

app.MapPost("/chat/sessions/{sessionId}/messages", (ChatManager chatManager, Guid sessionId, AddMessageRequest msg) =>
{
    var session = chatManager.GetSession(sessionId);
    if (session == null) { return Results.NotFound(); }
    session.AddUserMessage(msg.Message);
    return Results.Created();
});

app.MapPost("/chat/sessions/{sessionId}/run", async (ChatManager chatManager, Guid sessionId) =>
{
    var session = chatManager.GetSession(sessionId);
    if (session == null) { return Results.NotFound(); }
    return Results.Ok(await session.Run());
});

app.Run();

record AddMessageRequest(string Message);
