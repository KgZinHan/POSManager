using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace POSManager.Models
{
    public class Company
    {
        [Key][DisplayName("Company")] public string CmpyNme { get; set; } = string.Empty;

        [DisplayName("Group Name")] public string CmpyGrpNme { get; set; } = string.Empty;

        [DisplayName("Login")] public string LoginNme { get; set; } = string.Empty;

        [DisplayName("Address")] public string Address { get; set; } = string.Empty;

        [DisplayName("Create Datetime")] public DateTime CreateDteTime { get; set; }
    }
}
