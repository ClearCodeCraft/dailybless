using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

// Load configuration
var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json");
var config = builder.Build();

// 1. Generate email content from ChatGPT
var openAiClient = new HttpClient();
openAiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config["OpenAI:ApiKey"]);

var chatRequest = new
{
    model = "gpt-3.5-turbo",
    messages = new[]
    {
        new { role = "system", content = "You are an assistant that writes status update emails." },
        new { role = "user", content = "Write a brief daily status update email for a developer." }
    }
};

var chatJson = JsonSerializer.Serialize(chatRequest);
var chatContent = new StringContent(chatJson, Encoding.UTF8, "application/json");

var chatResponse = await openAiClient.PostAsync("https://api.openai.com/v1/chat/completions", chatContent);
var chatResponseJson = await chatResponse.Content.ReadAsStringAsync();
using var doc = JsonDocument.Parse(chatResponseJson);
var messageContent = doc.RootElement
    .GetProperty("choices")[0]
    .GetProperty("message")
    .GetProperty("content")
    .GetString();

// 2. Send the generated message via Brevo
var brevoClient = new HttpClient();
brevoClient.DefaultRequestHeaders.Add("api-key", config["Brevo:ApiKey"]);

var brevoEmail = new
{
    sender = new { name = config["Brevo:SenderName"], email = config["Brevo:SenderEmail"] },
    to = new[] { new { email = config["Brevo:RecipientEmail"] } },
    subject = "Your Daily Update 📬",
    htmlContent = $"<p>{messageContent?.Replace("\n", "<br>")}</p>"
};

var brevoJson = JsonSerializer.Serialize(brevoEmail);
var brevoContent = new StringContent(brevoJson, Encoding.UTF8, "application/json");

var brevoResponse = await brevoClient.PostAsync("https://api.brevo.com/v3/smtp/email", brevoContent);
Console.WriteLine(await brevoResponse.Content.ReadAsStringAsync());
