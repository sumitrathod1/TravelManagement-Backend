namespace TravelManagement.Models
{
    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProcessedFolder { get; set; }
        public int PollIntervalSeconds { get; set; }
        public string SubjectKeyword { get; set; }
    }
}
