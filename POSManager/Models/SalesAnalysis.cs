namespace POSManager.Models
{
    public class SalesAnalysis
    {
        public string BranchName { get; set; } = string.Empty;

        public DateTime FromDate { get; set; }

        public int NextDay { get; set; }

        public int TopBottom { get; set; }
    }
}
