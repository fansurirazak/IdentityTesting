using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityTesting.Models
{
    public class Program
    {
        public int ProgramID { get; set; }
        public string ProgramName { get; set; } = null!;

        public string ProgramCode { get; set; } = null!;
    }
}
