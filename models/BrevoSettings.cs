namespace DailyBlessingConsole.Models;

public class BrevoRecipient
{
    public string Email { get; set; }
    public string Name { get; set; }
}

public class BrevoSettings
{
    public string ApiKey { get; set; }
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public List<BrevoRecipient> Recipients { get; set; }
}
