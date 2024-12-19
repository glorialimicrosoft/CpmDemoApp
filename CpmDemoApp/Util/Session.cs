using CpmDemoApp.Models;

namespace CpmDemoApp.Util
{
    public static class Session
    {
        public static ApplicationUser LoggedInUser { get; set; } = new ApplicationUser();

        public static bool LoggedIn { get; set; } = false;
    }
}
