public async Task<string> GetDailyMessageAsync()
{
    var payload = new
    {
        model = "gpt-3.5-turbo-1106", // Cheapest & stable as of now
        messages = new[] {
            new { role = "user", content = "Write a short inspirational message for today." }
        }
    };

    var json = System.Text.Json.JsonSerializer.Serialize(payload);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
    var result = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine("OpenAI API error:");
        Console.WriteLine(result);
        return "Sorry, we couldn't fetch your blessing today. 🙏";
    }

    try
    {
        using var doc = System.Text.Json.JsonDocument.Parse(result);
        if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
        {
            return choices[0].GetProperty("message").GetProperty("content").GetString()
                   ?? "No content received.";
        }
        else
        {
            Console.WriteLine("OpenAI response didn't contain 'choices'.");
            Console.WriteLine(result);
            return "Sorry, we couldn't fetch your blessing today. 🙏";
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error parsing OpenAI response:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(result);
        return "Something went wrong. Please try again later.";
    }
}
