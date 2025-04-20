using DailyBlessingConsole.Services;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var ai = new AiMessageService(config);
var brevo = new BrevoEmailService(config);

Console.WriteLine("✨ Generating message...");
var message = await ai.GetDailyMessageAsync();
Console.WriteLine("✅ Message generated:\n" + message);

Console.WriteLine("📧 Sending emails...");
await brevo.SendBatchEmailAsync(message);
Console.WriteLine("✅ All emails sent.");
