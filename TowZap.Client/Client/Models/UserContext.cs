namespace TowZap.Client.Client.Models
{
    public class UserContext
    {
        public string Role { get; set; }
        public string Token { get; set; }
        public string FullName { get; set; }
        public Guid CompanyId { get; set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
        public void Clear()
        {
            Role = null;
            FullName = null;
            Token = null;
            CompanyId = Guid.Empty;
        }


    }
}
