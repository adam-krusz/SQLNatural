using OpenAI.GPT3.ObjectModels.RequestModels;

static class ChatMessagesHelper
{
    static public List<ChatMessage> PrepareChatMessages(DatabaseHelper databaseHelper)
    {
        List<ChatMessage> chatMessages = new()
        {
            ChatMessage.FromSystem("You are an SQL assistant. You convert the question to SQL and return only SQL.")
        };

        List<string> tableStructures = databaseHelper.RetrieveStructure();

        foreach (string tableStructure in tableStructures)
        {
            chatMessages.Add(ChatMessage.FromSystem(tableStructure));
        }
        chatMessages.Add(ChatMessage.FromUser("Answer only with SQL, dont send any other text."));
        return chatMessages;
    }
}