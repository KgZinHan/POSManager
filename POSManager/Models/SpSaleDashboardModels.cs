using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace POSManager.Models
{
    public class SpSaleDashboardModels
    {
    }

    [Keyless]
    public class SaleDashboardSpA0Model // Action 0
    {
        public string? Branch { get; set; }

        public decimal? SaleAmount { get; set; }
    }

    [Keyless]
    public class SaleDashboardSpA1n2Model // Action 1 and 2
    {
        public string? CurrCde { get; set; }

        public decimal? SaleAmount { get; set; }
    }

    [Keyless]
    public class SaleDashboardSpA3Model // Action 3
    {
        public DateTime? Bizdte { get; set; }

        public decimal? SaleAmount { get; set; }
    }

    [Keyless]
    public class SaleDashboardSpA4Model // Action 4
    {
        public string? ItemDesc { get; set; }

        public decimal? Qty { get; set; }
    }

    public class SaleDashboardDbModel
    {
        public int? No { get; set; }
        public string? Branch { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? SaleAmount { get; set; }

        public DateTime? Date { get; set; }

        public string? ItemDesc { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? Qty { get; set; }
    }

    public class SaleDashboardCurrencyDbModel
    {
        public int? No { get; set; }

        public string? CurrCde { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? SaleAmount { get; set; }
    }

}
