namespace POSManager.Models
{
    public class UploadAllData
    {
        public IEnumerable<Bill> BillList { get; set; } = new List<Bill>();

        public IEnumerable<BillP> BillPList { get; set; } = new List<BillP>();
    }
}
