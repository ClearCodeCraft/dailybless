using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DailyBlessingConsole.Services;

public class AiMessageService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public AiMessageService(IConfiguration config)
    {
        // Use environment variable for API Key (GitHub secret)
        _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? config["OpenAI:ApiKey"];
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> GetDailyMessageAsync()
    {
        var payload = new
        {
            model = "gpt-3.5-turbo",
            messages = new[] {
                new { role = "user", content = "Write a short inspirational message for today." }
            }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var result = await response.Content.ReadAsStringAsync();

        using var doc = System.Text.Json.JsonDocument.Parse(result);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
    }
}
