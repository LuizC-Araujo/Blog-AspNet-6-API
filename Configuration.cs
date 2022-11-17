namespace Blog;

public static class Configuration
{
    public static string JwtKey = "FC5EVHALfEqIRah1XMbhUw==";
    public static string ApiKeyName = "api_key";
    public static string ApiKey = "tDYO81qj6EC29aMYdzOiOQ==";
    public static SmtpConfiguration Smtp = new();

    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}