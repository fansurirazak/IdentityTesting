using Microsoft.AspNetCore.Identity;

namespace IdentityTesting.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; } = string.Empty;
        [PersonalData]
        public string LastName { get; set; } = string.Empty;
        [PersonalData]
        public string IC { get; set; } = string.Empty;
        public int UsernameChangeLimit { get; set; } = 10;
        public byte[]? ProfilePicture { get; set; }
    }
}
