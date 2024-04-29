using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace POSManager.Models
{
    public class SpYearlySalesModels
    {
    }

    [Keyless]
    public class YearlySalesSpA0Model
    {
        public string? CmpyGrpNme { get; set; }

        public string? Branch { get; set; }

        public int? YearNo { get; set; }

        public decimal? Amount { get; set; }

    }

    [Keyless]
    public class YearlySalesSpA1Model
    {
        public string? CatgCde { get; set; }

        public decimal? SaleAmount { get; set; }

    }

    public class YearlySalesModel
    {
        public int? No { get; set; }

        public string? Branch { get; set; }

        public int? YearNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? Amount { get; set; }

        public int? ProgressBar { get; set; }

    }

    public class YearlyCatgWiseSalesModel
    {
        public string? CatgCde { get; set; }

        public decimal? SaleAmount { get; set; }

    }

    public class YearlySalesHeadModel
    {
        public IEnumerable<YearlySalesModel> YearlySales { get; set; } = new List<YearlySalesModel>();

        public string? Branch { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? TotalAmount { get; set; }

    }
}
