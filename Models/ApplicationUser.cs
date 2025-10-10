using Microsoft.AspNetCore.Identity;

namespace AgriEnergyConnectPrototype.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Additional custom properties can be added here
        //Conenience property to get full name
        public string FullName => $"{FirstName} {LastName}";

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
