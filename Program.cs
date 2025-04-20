using DailyBlessingConsole.Services;
using Microsoft.Extensions.Configuration;
using System;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())  // Set the base path for configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)  // Add appsettings.json for local dev
    .AddEnvironmentVariables()  // Add environment variables (for GitHub Actions secrets)
    .Build();

// Read environment variables from GitHub Actions secrets
var ai = new AiMessageService(config);
var brevo = new BrevoEmailService(config);

Console.WriteLine("✨ Generating message...");
var message = await ai.GetDailyMessageAsync();
Console.WriteLine("✅ Message generated:\n" + message);

Console.WriteLine("📧 Sending emails...");
await brevo.SendBatchEmailAsync(message);
Console.WriteLine("✅ All emails sent.");
