using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityTesting.Models
{

    public class ACADDomain
    {
        [DisplayName("Domain ID")]

        public int ACADDomainID { get; set; }
        [DisplayName("Domain Name")]

        public string ACADDomainName { get; set; }
    }
}
