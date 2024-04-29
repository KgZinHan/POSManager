using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace POSManager.Models
{
    public class SpSaleAnalysisModels
    {

    }

    [Keyless]
    public class SaleAnalysisSpA0Model // Action 0
    {
        public string? ItemDesc { get; set; }

        public decimal? SaleAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? Qty { get; set; }
    }

    public class SaleAnalysisDbModel
    {
        public int? No { get; set; }

        public string? ItemDesc { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? Qty { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? SaleAmount { get; set; }
    }
}
