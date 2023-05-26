using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;


string dbFilePath = "C:\\Users\\adam.kruszewski\\sqlite\\chinook.db";
DatabaseHelper databaseHelper = new DatabaseHelper(dbFilePath);
List<string> tablesStructure = databaseHelper.RetrieveStructure();
foreach (string tableStructure in tablesStructure)
{
    //  Console.WriteLine($"{j++}: {tableStructure}");
}

var openAiService = new OpenAIService(new OpenAiOptions()
{
    //read ApiKey from text file
    ApiKey = File.ReadAllText("openai-api-key.txt")


});

// const string EndOfDatabaseStructure = "<<< END OF DATABASE STRUCTURE >>>";
const string SQLOnlyRemark = "Answer only with SQL, dont send any other text.";

List<ChatMessage> initialMessages = new()
    {
        ChatMessage.FromSystem("You are an SQL assistant. You convert the question to SQL and return only SQL."),

    };

foreach (string tableStructure in tablesStructure)
{
    initialMessages.Add(ChatMessage.FromUser(tableStructure));
    
}

// initialMessages.Add(ChatMessage.FromUser(EndOfDatabaseStructure));

while (true)
{
    Console.WriteLine("Enter a query in natural language (type '/q' or '/quit' to exit):");
    string? input = Console.ReadLine();

    if (input == "/q" || input == "/quit")
    {
        break; // Exit the loop
    }
    else if (string.IsNullOrEmpty(input))
    {
        continue; // Wait for input again
    }
    Console.WriteLine();
    var messages = initialMessages.ToList();
    messages.Add(ChatMessage.FromUser(input + ". " + SQLOnlyRemark));

    Console.WriteLine();
    
    var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
    {
        Messages = messages,
        Model = Models.ChatGpt3_5Turbo,
        Temperature = 0.0f,
    });

    

    if (completionResult.Error != null)
    {
        Console.WriteLine(completionResult.Error.Message);
        continue;
    }

    if (completionResult.Successful)
    {
        Console.WriteLine($"Total tokens: {completionResult.Usage.TotalTokens}, prompt tokens {completionResult.Usage.PromptTokens} completion tokens: {completionResult.Usage.CompletionTokens}");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(completionResult.Choices.First().Message.Content);
        Console.ResetColor();

    }

    Console.WriteLine();

    if (completionResult.Choices.First().Message.Content.ToUpper().StartsWith("SELECT"))
    {
        databaseHelper.DisplayQueryResults(completionResult.Choices.First().Message.Content);
    
    }

    


}



