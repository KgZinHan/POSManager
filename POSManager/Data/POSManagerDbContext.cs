using Microsoft.EntityFrameworkCore;
using POSManager.Models;

namespace POSManager.Data
{
    public class POSManagerDbContext : DbContext
    {
        public POSManagerDbContext(DbContextOptions<POSManagerDbContext> options) : base(options) { }

        /*Common*/
        public DbSet<CompanyUser> ms_companyuser { get; set; }

        public DbSet<Company> ms_company { get; set; }

        public DbSet<Bill> pmgr_bill { get; set; }

        public DbSet<BillP> pmgr_billp { get; set; }

        // SaleDashboard

        public DbSet<SaleDashboardSpA0Model> spSaleDashboardDbA0Set { get; set; }

        public DbSet<SaleDashboardSpA1n2Model> spSaleDashboardDbA1n2Set { get; set; }

        public DbSet<SaleDashboardSpA3Model> spSaleDashboardDbA3Set { get; set; }

        public DbSet<SaleDashboardSpA4Model> spSaleDashboardDbA4Set { get; set; }

        // Monthly Sales

        public DbSet<MonthlySalesSpA0n1Model> spMonthlySalesDbA0n1Set { get; set; }

        public DbSet<MonthlySalesSpA2Model> spMonthlySalesDbA2Set { get; set; }

        // Yearly Sales

        public DbSet<YearlySalesSpA0Model> spYearlySalesDbA0Set { get; set; }

        public DbSet<YearlySalesSpA1Model> spYearlySalesDbA1Set { get; set; }

        // Sale Analysis

        public DbSet<SaleAnalysisSpA0Model> spSaleAnalysisDbA0Set { get; set; }

    }
}
