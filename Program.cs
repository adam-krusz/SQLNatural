using OpenAI.GPT3.ObjectModels.RequestModels;

OpenAi openAi = new ("openai-api-key.txt");
string dbFilePath = "chinook.db";
DatabaseHelper databaseHelper = new (dbFilePath);
var chatMessages = ChatMessagesHelper.PrepareChatMessages(databaseHelper);

while (true)
{
    Console.WriteLine("Enter a query in natural language (type '/q' or '/quit' to exit):");

    string? input = args.Length > 0 ? string.Join(" ", args) : Console.ReadLine();

    if (input == "/q" || input == "/quit")
    {
        break; // Exit the loop
    }
    else if (string.IsNullOrEmpty(input))
    {
        continue; // Wait for input again
    }
    Console.WriteLine();

    chatMessages.Add(ChatMessage.FromUser(input));

    Console.WriteLine();

    var completionResult = await openAi.GetCompletionResults(chatMessages);

    if (completionResult.Error != null)
    {
        Console.WriteLine(completionResult.Error.Message);
        continue;
    }

    if (completionResult.Successful)
    {
        Console.WriteLine($"Total tokens: {completionResult.Usage.TotalTokens}, prompt tokens {completionResult.Usage.PromptTokens} completion tokens: {completionResult.Usage.CompletionTokens}");
        Console.WriteLine();
        Console.WriteLine(completionResult.Choices.First().Message.Content);
    }

    Console.WriteLine();

    if (completionResult.Choices.First().Message.Content.ToUpper().StartsWith("SELECT"))
    {
        databaseHelper.DisplayQueryResults(completionResult.Choices.First().Message.Content);

    }

    if (args.Length > 0)
        break;
}

