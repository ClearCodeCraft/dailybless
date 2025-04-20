using System.Net.Http.Headers;
using System.Text;
using DailyBlessingConsole.Models;
using Microsoft.Extensions.Configuration;

namespace DailyBlessingConsole.Services;

public class BrevoEmailService
{
    private readonly HttpClient _httpClient;
    private readonly BrevoSettings _settings;

    public BrevoEmailService(IConfiguration config)
    {
        // Use environment variable for API Key (GitHub secret)
        var apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY") ?? config["Brevo:ApiKey"];

        _settings = config.GetSection("Brevo").Get<BrevoSettings>();
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("api-key", apiKey); // Use the API key from the environment variable
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task SendBatchEmailAsync(string message)
    {
        var messageVersions = _settings.Recipients.Select(r => new
        {
            to = new[] { new { email = r.Email, name = r.Name } },
            htmlContent = $"<html><body><h3>Hi {r.Name},</h3><p>{message.Replace("\n", "<br>")}</p></body></html>",
            subject = "🌞 Your Daily Blessing"
        }).ToArray();

        var payload = new
        {
            sender = new
            {
                name = _settings.SenderName,
                email = _settings.SenderEmail
            },
            subject = "Default Subject (Will be overridden)",
            htmlContent = "<html><body>This will be overridden</body></html>",
            messageVersions
        };

        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var response = await _httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", new StringContent(json, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }
}
