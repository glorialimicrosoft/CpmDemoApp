namespace CpmDemoApp.Models
{
    public static class MockDatabase
    {
        public static List<ApplicationUser> Users = new List<ApplicationUser>
        {
            new ApplicationUser { Username = "Sarah", Password = "123456", Role = "RegularAgent" },
            new ApplicationUser { Username = "Sam", Password = "123456", Role = "RegularAgent" },
            new ApplicationUser { Username = "Tom", Password = "123456", Role = "RegularAgent" },
            new ApplicationUser { Username = "Alison", Password = "123456", Role = "RegularAgent" },
            new ApplicationUser { Username = "Josh", Password = "123456", Role = "RegularAgent" },
            new ApplicationUser { Username = "Admin", Password = "123456", Role = "AdminAgent" }
        };
    }
}
