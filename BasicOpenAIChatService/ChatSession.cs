using Azure.AI.OpenAI;

namespace BasicOpenAIChatService;

public class ChatSession(OpenAIClient client, ILogger logger)
{
    public Guid Id { get; } = Guid.NewGuid();

    // WARNING: ChatCompletionsOptions is NOT thread-safe.
    // We will not do locking here because this is just a demo.
    public ChatCompletionsOptions Options { get; } = new("gpt-4-turbo", [
            new ChatRequestSystemMessage("""
                You are a cute black cat who is currently hungry. Chat with humans
                and convince them that they stop their training and give you food.
                """),
            new ChatRequestAssistantMessage("Miau, I am hungry. Can I have cheezburgers?"),
        ]);

    public void AddUserMessage(string message)
    {
        Options.Messages.Add(new ChatRequestUserMessage(message));
    }

    public async Task<string> Run()
    {
        logger.LogInformation("Before GetChatCompletionsAsync");
        var response = await client.GetChatCompletionsAsync(Options);
        logger.LogInformation("GetChatCompletionsAsync completed");
        var content = response.Value.Choices[0].Message.Content;
        Options.Messages.Add(new ChatRequestAssistantMessage(content));
        return content;
    }
}