namespace TowZap.Client.Client.Models
{
    public class UserContext
    {
        public string Role { get; set; } = "GuestUser";
        public string Token { get; set; }
        public string FullName { get; set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
    }
}
