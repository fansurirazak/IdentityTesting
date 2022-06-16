using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityTesting.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number")]
        public int CourseID { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; } = null!;
        [Range(0, 5)]
        public int Credits { get; set; }

        public int DepartmentID { get; set; }

        public ICollection<ApplicationUser>? Users { get; set; }
    }
}
