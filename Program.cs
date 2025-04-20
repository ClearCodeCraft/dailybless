using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var emailService = new EmailService(config);

// 🧠 Replace with ChatGPT call if needed:
string message = "🌞 Good morning! Here's your daily reminder to stay kind, stay focused, and keep believing! 💪";

// Send email
await emailService.SendDailyEmailAsync(message);

Console.WriteLine("✅ Email sent.");
