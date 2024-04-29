using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace POSManager.Models
{
    public class SpMonthlySalesModels
    {
    }

    [Keyless]
    public class MonthlySalesSpA0n1Model
    {
        public string? CmpyGrpNme { get; set; }

        public string? Branch { get; set; }

        public int? MonthNo { get; set; }

        public decimal? Amount { get; set; }

    }

    [Keyless]
    public class MonthlySalesSpA2Model
    {
        public string? CatgCde { get; set; }

        public decimal? SaleAmount { get; set; }

    }

    public class MonthlySalesModel
    {
        public int? No { get; set; }

        public string? Branch { get; set; }

        public int? MonthNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? Amount { get; set; }

        public string? Month { get; set; }

        public int? ProgressBar { get; set; }

    }

    public class CatgWiseSalesModel
    {
        public string? CatgCde { get; set; }

        public decimal? SaleAmount { get; set; }

    }

    public class MonthlySalesHeadModel
    {
        public IEnumerable<MonthlySalesModel> MonthlySales { get; set; } = new List<MonthlySalesModel>();

        public string? Branch { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")] public decimal? TotalAmount { get; set; }

    }
}
