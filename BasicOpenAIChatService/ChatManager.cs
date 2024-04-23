using System.Collections.Concurrent;
using Azure.AI.OpenAI;

namespace BasicOpenAIChatService;

public class ChatManager(OpenAIClient client, ILogger<ChatManager> logger)
{
    private ConcurrentDictionary<Guid, ChatSession> Sessions { get; } = [];

    public ChatSession CreateSession()
    {
        var session = new ChatSession(client, logger);
        Sessions[session.Id] = session;
        logger.LogInformation("Created session {Id}", session.Id);
        return session;
    }

    public ChatSession? GetSession(Guid id)
    {
        if (Sessions.TryGetValue(id, out var session))
        {
            return session;
        }

        return null;
    }
}
