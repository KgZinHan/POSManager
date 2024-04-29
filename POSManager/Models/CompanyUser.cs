using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace POSManager.Models
{
    public class CompanyUser
    {
        [Key][DisplayName("Login")] public string LoginNme { get; set; } = string.Empty;

        [DisplayName("Password")] public string Pwd { get; set; } = string.Empty;

        [DisplayName("Active Flag?")] public bool ActiveFlg { get; set; }

        [DisplayName("Create Datetime")] public DateTime CreateDteTime { get; set; }
    }
}
