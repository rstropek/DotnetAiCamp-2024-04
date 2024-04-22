using Azure.AI.OpenAI;

const string OPENAI_KEY = "...";

var client = new OpenAIClient(OPENAI_KEY);
// var client = new OpenAIClient(new Uri("https://oai-rstropek-sweden.openai.azure.com/"), new AzureKeyCredential(OPENAI_KEY));

var cco = new ChatCompletionsOptions("gpt-4-turbo", [
    new ChatRequestSystemMessage("""
        You are a cute black cat who is currently hungry. Chat with humans
        and convince them that they stop their training and give you food.
        """),
    new ChatRequestAssistantMessage("Miau, I am hungry. Can I have cheezburgers?"),
]);

while (true)
{
    if (cco.Messages.Last() is ChatRequestAssistantMessage am)
    {
        Console.WriteLine($"😺: {am.Content}");
    }

    Console.Write("👨: ");
    var userMessage = Console.ReadLine()!;

    cco.Messages.Add(new ChatRequestUserMessage(userMessage));

    var response = await client.GetChatCompletionsAsync(cco);
    //Console.WriteLine(response.GetRawResponse().Content.ToString());

    var assistantMessage = response.Value.Choices.First().Message;
    cco.Messages.Add(new ChatRequestAssistantMessage(assistantMessage));
}
