using DailyBlessingConsole.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Set up the configuration
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Set up Dependency Injection
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton<AiMessageService>()
            .AddSingleton<BrevoEmailService>()
            .BuildServiceProvider();

        // Get services
        var aiMessageService = serviceProvider.GetRequiredService<AiMessageService>();
        var brevoEmailService = serviceProvider.GetRequiredService<BrevoEmailService>();

        // Generate the daily message and send emails
        Console.WriteLine("✨ Generating message...");
        var message = await aiMessageService.GetDailyMessageAsync();
        Console.WriteLine("✅ Message generated:\n" + message);

        Console.WriteLine("📧 Sending emails...");
        await brevoEmailService.SendBatchEmailAsync(message);
        Console.WriteLine("✅ All emails sent.");
    }
}
