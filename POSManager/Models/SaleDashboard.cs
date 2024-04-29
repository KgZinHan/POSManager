using System.ComponentModel.DataAnnotations;

namespace POSManager.Models
{
    public class SaleDashboard
    {
        public string BranchName { get; set; } = string.Empty;

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }

    public class SaleDashboardModels
    {

        public IEnumerable<SaleDashboardDbModel> SaleDashboardList { get; set; } = new List<SaleDashboardDbModel>();

        public IEnumerable<SaleDashboardCurrencyDbModel> SaleCurrList { get; set; } = new List<SaleDashboardCurrencyDbModel>();

        public SaleDashboard SaleDashboard { get; set; } = new SaleDashboard();

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? TotalAmt { get; set; }
    }
}
