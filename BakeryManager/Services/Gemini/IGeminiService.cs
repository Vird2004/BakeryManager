namespace BakeryManager.Services.Gemini
{
    public interface IGeminiService
    {
        Task<string> GetAIResponse(string input);
    }
}
