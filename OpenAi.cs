using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels.ResponseModels;

public class OpenAi
{
    private OpenAIService openAiService = default!;
    private string apiFileName ="";
    string model = Models.ChatGpt3_5Turbo;
    float temperature = 0.0f;

     public OpenAi(string apiKeyFileName) 
    {
        this.apiFileName = apiKeyFileName;
        this.model = model =="" ? Models.ChatGpt3_5Turbo : model;
        openAiService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = GetApiKey()
        });
    }
    //TODO use strategy pattern
    public string GetApiKey() => File.ReadAllText(apiFileName);

    public async Task<ChatCompletionCreateResponse> GetCompletionResults(List<ChatMessage> messages)
    {
        var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = model,
            Temperature = temperature,
        });
        return completionResult;
    }
}
