using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using POSManager.Data;
using POSManager.Models;

namespace POSManager.Controllers
{
    [Authorize]
    public class SaleDashboardController : Controller
    {
        private readonly POSManagerDbContext _context;

        public SaleDashboardController(POSManagerDbContext context)
        {
            _context = context;
        }

        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            var loginName = GetLoginName();

            var saleDashboard = new SaleDashboard()
            {
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                ToDate = DateTime.Now
            };

            ViewData["BranchNames"] = new SelectList(_context.ms_company.Where(cmpy => cmpy.LoginNme == loginName), "CmpyNme", "CmpyNme");

            var list = new SaleDashboardModels()
            {
                SaleDashboard = saleDashboard,
                SaleDashboardList = new List<SaleDashboardDbModel>()
            };

            return View(list);
        }

        [HttpPost]
        public IActionResult Search(string branchName, DateTime fromDate, DateTime toDate)
        {
            SetLayOutData();

            var loginName = GetLoginName();
            var cmpygrpnme = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyGrpNme).FirstOrDefault();
            var saleDashboardAction = 0;
            var currDashboardAction = 1;

            // SaleDashboardList

            var saleDashboardList = _context.spSaleDashboardDbA0Set
                        .FromSqlRaw("EXEC sp_saledashboard @action={0}, @cmpygrpnme = {1}, @fromdate = {2}, @todate = {3}", saleDashboardAction, cmpygrpnme, fromDate, toDate)
                        .AsEnumerable()
                        .Select((x, Index) => new SaleDashboardDbModel
                        {
                            Branch = x.Branch,
                            SaleAmount = x.SaleAmount
                        })
                        .ToList();

            if (!branchName.IsNullOrEmpty())
            {
                saleDashboardList = saleDashboardList.Where(list => list.Branch == branchName).ToList();
                currDashboardAction = 2;
            }

            foreach (var item in saleDashboardList)
            {
                item.No = saleDashboardList.IndexOf(item) + 1;
            }

            // CurrencySaleDashboardList

            var currDashboardList = _context.spSaleDashboardDbA1n2Set
                        .FromSqlRaw("EXEC sp_saledashboard @action={0}, @cmpygrpnme = {1}, @fromdate = {2}, @todate = {3},@cmpynme = {4}", currDashboardAction, cmpygrpnme, fromDate, toDate, branchName)
                        .AsEnumerable()
                        .Select((x, Index) => new SaleDashboardCurrencyDbModel
                        {
                            CurrCde = x.CurrCde,
                            SaleAmount = x.SaleAmount
                        })
                        .ToList();

            foreach (var item in currDashboardList)
            {
                item.No = currDashboardList.IndexOf(item) + 1;
            }

            // SaleDashboard

            var saleDashboard = new SaleDashboard()
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            ViewData["BranchNames"] = new SelectList(_context.ms_company.Where(cmpy => cmpy.LoginNme == loginName), "CmpyNme", "CmpyNme");

            var list = new SaleDashboardModels()
            {
                SaleDashboard = saleDashboard,
                SaleDashboardList = saleDashboardList,
                SaleCurrList = currDashboardList,
                TotalAmt = saleDashboardList.Sum(list => list.SaleAmount)
            };

            return PartialView("_SaleDashboardSearchPartial", list);
        }

        public IActionResult DateWiseSales(string name, DateTime fromDate, DateTime toDate)
        {
            SetLayOutData();

            var loginName = GetLoginName();
            var dateWiseSaleAction = 3;
            var cmpygrpnme = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyGrpNme).FirstOrDefault();

            var dateWiseSaleList = _context.spSaleDashboardDbA3Set
                        .FromSqlRaw("EXEC sp_saledashboard @action={0}, @cmpynme = {1}, @fromdate = {2}, @todate = {3}", dateWiseSaleAction, name, fromDate, toDate)
                        .AsEnumerable()
                        .Select(x => new SaleDashboardDbModel
                        {
                            Date = x.Bizdte,
                            SaleAmount = x.SaleAmount
                        })
                        .ToList();

            foreach (var item in dateWiseSaleList)
            {
                item.No = dateWiseSaleList.IndexOf(item) + 1;
            }

            ViewData["BranchName"] = name;

            return View(dateWiseSaleList);
        }

        public IActionResult ItemWiseSales(string name, DateTime date)
        {
            SetLayOutData();

            var loginName = GetLoginName();
            var itemWiseSaleAction = 4;
            var cmpygrpnme = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyGrpNme).FirstOrDefault();

            var itemWiseSaleList = _context.spSaleDashboardDbA4Set
                        .FromSqlRaw("EXEC sp_saledashboard @action={0}, @cmpynme = {1}, @thisdate = {2}", itemWiseSaleAction, name, date)
                        .AsEnumerable()
                        .Select(x => new SaleDashboardDbModel
                        {
                            ItemDesc = x.ItemDesc,
                            Qty = x.Qty
                        })
                        .ToList();

            foreach (var item in itemWiseSaleList)
            {
                item.No = itemWiseSaleList.IndexOf(item) + 1;
            }

            ViewData["BranchName"] = name;

            ViewData["Date"] = date.ToString("dd MMM yyyy");

            return View(itemWiseSaleList);
        }

        #endregion


        #region // Global methods (Important)//


        protected string GetLoginName()
        {
            var loginName = HttpContext.User.Claims.FirstOrDefault()?.Value;
            return loginName ?? "";
        }

        protected void SetLayOutData()
        {
            var loginName = HttpContext.User.Claims.FirstOrDefault()?.Value;

            if (loginName == null)
            {
                loginName = "";
            }

            if (loginName.ToUpper() == "ADMIN")
            {
                ViewData["Role"] = "ACCESS_LEVEL";
            }

            ViewData["Username"] = loginName;
        }


        #endregion


    }
}
