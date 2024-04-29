using System.ComponentModel.DataAnnotations;

namespace POSManager.Models
{
    public class Bill
    {
        [Key] public int BillD { get; set; }

        public string CmpyNme { get; set; } = string.Empty;

        public DateTime Bizdte { get; set; }

        public string ItemId { get; set; } = string.Empty;

        public string? ItemDesc { get; set; }

        public string CatgCde { get; set; } = string.Empty;

        public decimal Qty { get; set; }

        public decimal Amount { get; set; }
    }
}
