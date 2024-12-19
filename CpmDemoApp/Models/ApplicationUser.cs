namespace CpmDemoApp.Models
{
    public class ApplicationUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "RegularAgent" or "AdminAgent"
    }
}
