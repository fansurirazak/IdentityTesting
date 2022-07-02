using System.ComponentModel.DataAnnotations;

namespace IdentityTesting.Models.ViewModels
{
    public class LecturerDomainViewModel
    {
        public string Id { get; set; } = null!;
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        public string IC { get; set; } = string.Empty;
        public string Matric { get; set; } = null!;

        public string Email { get; set; }

        public string Role { get; set; }

        public string? ACADDomainName { get; set; }
    }
}
