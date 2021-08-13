namespace SUShirts.Configuration
{
    public class MessageOptions
    {
        public string DiscordWebhookUrl { get; set; }

        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; } = 465;
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool UseSsl { get; set; } = true;
        public string FromAddress { get; set; }
        public string ReservationManagerAddress { get; set; }
    }
}
