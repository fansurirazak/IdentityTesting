using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityTesting.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [DisplayName("First Name")]

        public string FirstName { get; set; } = string.Empty;
        [PersonalData]
        [DisplayName("Last Name")]

        public string LastName { get; set; } = string.Empty;
        [PersonalData]
        [DisplayName("IC")]
        public string IC { get; set; } = string.Empty;

        public int UsernameChangeLimit { get; set; } = 10;
        public byte[]? ProfilePicture { get; set; }
        [DisplayName("Matric Number")]
        public string? Matric { get; set; } = string.Empty;
        [DisplayName("Semester")]
        public string Semester  { get; set; } = string.Empty;
        [DisplayName("Session")]
        public string Session { get; set; } = string.Empty;

        public ACADDomain? Domain { get; set; }
        [DisplayName("Domain Name")]
        public int? DomainID { get; set; }

        public ACDProgram? Program { get; set; }
        [DisplayName("Program Name")]
        public int? ProgramID { get; set; }
    }
}
