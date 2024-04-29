namespace POSManager.Models
{
    public class BillP
    {
        public int BillPId { get; set; }

        public string CmpyNme { get; set; } = string.Empty;

        public DateTime BizDte { get; set; }

        public string CurrCde { get; set; } = string.Empty;

        public decimal Amt { get; set; }
    }
}
