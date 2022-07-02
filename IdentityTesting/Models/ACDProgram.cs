using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityTesting.Models
{
    public class ACDProgram
    {
        [DisplayName("Program ID")]
        public int ACDProgramID { get; set; }
        [DisplayName("Program Name")]
        public string ACDProgramName { get; set; } = null!;
        [DisplayName("Program Code")]
        public string ACDProgramCode { get; set; } = null!;
    }
}
