using System.Net.Http;
using System.Net.Http.Headers;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.Json;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        var openAiApiKey = Environment.GetEnvironmentVariable("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");
        var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var fromEmail = "your_verified_email@domain.com";
        var toEmail = "recipient@domain.com";

        var chatGptMessage = await GetChatGptMessage(openAiApiKey);
        await SendEmail(chatGptMessage, sendGridApiKey, fromEmail, toEmail);
    }

    static async Task<string> GetChatGptMessage(string apiKey)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "user", content = "Write me a short motivational email for the team." }
            },
            temperature = 0.8
        };

        var response = await client.PostAsync(
            "https://api.openai.com/v1/chat/completions",
            new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        );

        var result = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
        return result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
    }

    static async Task SendEmail(string body, string apiKey, string from, string to)
    {
        var client = new SendGridClient(apiKey);
        var msg = MailHelper.CreateSingleEmail(
            new EmailAddress(from, "LLR AI Bot"),
            new EmailAddress(to),
            "🤖 Your Daily AI Email",
            body,
            $"<strong>{body}</strong>"
        );

        var response = await client.SendEmailAsync(msg);
        Console.WriteLine($"Email sent: {response.StatusCode}");
    }
}
