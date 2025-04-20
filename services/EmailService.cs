using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public EmailService(IConfiguration config)
    {
        _config = config;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("api-key", _config["Brevo:ApiKey"]);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task SendDailyEmailAsync(string messageContent)
    {
        var email = new
        {
            sender = new {
                name = _config["Brevo:SenderName"],
                email = _config["Brevo:SenderEmail"]
            },
            to = new[] {
                new { email = _config["Brevo:RecipientEmail"] }
            },
            subject = "Your Daily Update 📬",
            htmlContent = $"<p>{messageContent.Replace("\n", "<br>")}</p>"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(email);
        var response = await _httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", new StringContent(json, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();
    }
}
