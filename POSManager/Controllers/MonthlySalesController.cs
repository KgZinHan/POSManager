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
    public class MonthlySalesController : Controller
    {
        private readonly POSManagerDbContext _context;

        public MonthlySalesController(POSManagerDbContext context)
        {
            _context = context;
        }

        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            var loginName = GetLoginName();
            var cmpyNme = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyNme).FirstOrDefault();

            var monthlySales = new MonthlySales()
            {
                Year = DateTime.Now.Year
            };

            ViewData["BranchNames"] = new SelectList(_context.ms_company.Where(cmpy => cmpy.LoginNme == loginName), "CmpyNme", "CmpyNme");

            var categories = _context.pmgr_bill
                .GroupBy(bill => bill.CatgCde)
                .Select(group => group.Key) 
                .Distinct()
                .ToList();

            ViewData["Categories"] = new SelectList(categories);

            return View(monthlySales);
        }

        [HttpPost]
        public IActionResult View(string branchName, string catgCde, int year)
        {
            var loginName = GetLoginName();
            var cmpygrpnme = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyGrpNme).FirstOrDefault();
            var monthlySalesAction = 0;

            if (!catgCde.IsNullOrEmpty())
            {
                monthlySalesAction = 1;
            }

            var mainList = new List<MonthlySalesHeadModel>();

            var monthlySales = _context.spMonthlySalesDbA0n1Set
                    .FromSqlRaw("EXEC sp_monthlySales @action={0}, @cmpygrpnme = {1}, @year = {2},@catgcde = {3}", monthlySalesAction, cmpygrpnme, year, catgCde)
                    .AsEnumerable()
                    .Select(x => new MonthlySalesModel
                    {
                        Branch = x.Branch,
                        MonthNo = x.MonthNo,
                        Amount = x.Amount
                    })
                    .ToList();

            if (!branchName.IsNullOrEmpty()) // branchname is there
            {
                var newMonthlySales = monthlySales.Where(list => list.Branch == branchName).ToList();

                var totalAmt = newMonthlySales.Sum(list => list.Amount);

                foreach (var item in newMonthlySales)
                {
                    item.No = monthlySales.IndexOf(item) + 1;
                    item.Month = GetMonth(item.MonthNo ?? 0);
                    item.ProgressBar = GetProgressBar(item.Amount ?? 0, totalAmt ?? 0);
                }

                var headModel = new MonthlySalesHeadModel()
                {
                    MonthlySales = newMonthlySales,
                    Branch = branchName,
                    TotalAmount = totalAmt
                };

                mainList.Add(headModel);
            }
            else // select all branches
            {
                var allBranches = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyNme).ToList();

                foreach (var branch in allBranches)
                {
                    var newMonthlySales = monthlySales.Where(x => x.Branch == branch).ToList();
                    var totalAmt = newMonthlySales.Sum(list => list.Amount);

                    foreach (var item in newMonthlySales)
                    {
                        item.No = monthlySales.IndexOf(item) + 1;
                        item.Month = GetMonth(item.MonthNo ?? 0);
                        item.ProgressBar = GetProgressBar(item.Amount ?? 0, totalAmt ?? 0);
                    }

                    var headModel = new MonthlySalesHeadModel()
                    {
                        MonthlySales = newMonthlySales,
                        Branch = branch,
                        TotalAmount = totalAmt
                    };
                    mainList.Add(headModel);
                }
            }

            return PartialView("_MonthlySalesViewPartial", mainList);
        }

        public IActionResult GetDonutChartData(string branchName, string catgCde, int year)
        {
            var loginName = GetLoginName();
            var cmpygrpnme = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyGrpNme).FirstOrDefault();
            var monthlySalesAction = 0;

            if (!catgCde.IsNullOrEmpty()) // for specific category only
            {
                monthlySalesAction = 1;
            }


            if (branchName.IsNullOrEmpty()) // for all branches
            {

                var allBranches = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyNme).ToList();

                // Add chartData labels
                var newLabels = new List<string>();
                newLabels.AddRange(allBranches);

                // Add chartData datasets data
                var monthlySales = _context.spMonthlySalesDbA0n1Set
                    .FromSqlRaw("EXEC sp_monthlySales @action={0}, @cmpygrpnme = {1}, @year = {2},@catgcde = {3}", monthlySalesAction, cmpygrpnme, year, catgCde)
                    .AsEnumerable()
                    .Select(x => new MonthlySalesModel
                    {
                        Branch = x.Branch,
                        Amount = x.Amount
                    })
                    .ToList();

                var newData = new List<int>();

                foreach (var branch in allBranches)
                {
                    var totalAmt = (int)(monthlySales.Where(x => x.Branch == branch).Sum(x => x.Amount) ?? 0);
                    newData.Add(totalAmt);
                }

                var chartData = new
                {
                    labels = newLabels.ToArray(),
                    datasets = new[]
                        {
                        new
                        {
                            data = newData.ToArray(),
                            backgroundColor = new[] { "#f56954", "#00a65a" ,"#f39c12", "#00c0ef", "#3c8dbc", "#d2d6de","#9b59b6","#3498db","#e74c3c","#2ecc71","#1abc9c","#34495e" }
                        }
                    }
                };
                return Json(chartData);
            }
            else // for specific branch only
            {
                // Add chartData labels
                var newLabel = new List<string> { branchName };

                // Add chartData datasets data
                var monthlySales = _context.spMonthlySalesDbA0n1Set
                    .FromSqlRaw("EXEC sp_monthlySales @action={0}, @cmpygrpnme = {1}, @year = {2},@catgcde = {3}", monthlySalesAction, cmpygrpnme, year, catgCde)
                    .AsEnumerable()
                    .Select(x => new MonthlySalesModel
                    {
                        Branch = x.Branch,
                        Amount = x.Amount
                    })
                    .Where(x => x.Branch == branchName)
                    .ToList();

                var totalAmt = (int)(monthlySales.Sum(x => x.Amount) ?? 0);
                var newData = new List<int> { totalAmt };

                var chartData = new
                {
                    labels = newLabel.ToArray(),
                    datasets = new[]
                        {
                        new
                        {
                            data = newData.ToArray(),
                            backgroundColor = new[] { "#f56954", "#00a65a" ,"#f39c12", "#00c0ef", "#3c8dbc", "#d2d6de","#9b59b6","#3498db","#e74c3c","#2ecc71","#1abc9c","#34495e" }
                        }
                    }
                };
                return Json(chartData);
            }
        }

        public IActionResult GetBarChartData(string branchName, string catgCde, int year)
        {
            var loginName = GetLoginName();
            var cmpygrpnme = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyGrpNme).FirstOrDefault();

            // both null or empty
            var catgSalesAction = 2;

            var categories = _context.pmgr_bill
                    .Where(bill => bill.Bizdte.Year == year)
                    .GroupBy(bill => bill.CatgCde)
                    .Select(group => group.Key)
                    .Distinct()
                    .ToList();

            if (!branchName.IsNullOrEmpty() && catgCde.IsNullOrEmpty())
            {
                catgSalesAction = 3;
                categories = _context.pmgr_bill
                   .Where(bill => bill.CmpyNme == branchName && bill.Bizdte.Year == year)
                   .GroupBy(bill => bill.CatgCde)
                   .Select(group => group.Key)
                   .Distinct()
                   .ToList();
            }

            if (branchName.IsNullOrEmpty() && !catgCde.IsNullOrEmpty())
            {
                catgSalesAction = 4;
                categories = new List<string>()
                {
                    catgCde
                };
            }

            if (!branchName.IsNullOrEmpty() && !catgCde.IsNullOrEmpty())
            {
                catgSalesAction = 5;
                categories = new List<string>()
                {
                    catgCde
                };
            }


            // Add chartData labels
            var newLabels = new List<string>();
            newLabels.AddRange(categories);

            // Add chartData datasets data
            var catgWiseSales = _context.spMonthlySalesDbA2Set
                .FromSqlRaw("EXEC sp_monthlySales @action={0}, @cmpygrpnme = {1}, @year = {2},@catgcde = {3},@cmpynme = {4}", catgSalesAction, cmpygrpnme, year, catgCde,branchName)
                .AsEnumerable()
                .Select(x => new CatgWiseSalesModel
                {
                    CatgCde = x.CatgCde,
                    SaleAmount = x.SaleAmount
                })
                .ToList();

            var newData = new List<int>();

            foreach (var catg in catgWiseSales)
            {
                var totalAmt = (int)(catg.SaleAmount ?? 0);
                newData.Add(totalAmt);
            }

            var chartData = new
            {
                labels = newLabels.ToArray(),
                datasets = new[]
                    {
                        new
                        {
                            axis = "y",
                            label = "Categories",
                            backgroundColor = "rgba(60,141,188,0.9)",
                            borderColor = "rgba(60,141,188,0.8)",
                            pointRadius =  false,
                            pointColor = "#3b8bba",
                            pointStrokeColor = "rgba(60,141,188,1)",
                            pointHighlightFill = "#fff",
                            pointHighlightStroke = "rgba(60,141,188,1)",
                            data = newData.ToArray()
                        }
                    }
            };
            return Json(chartData);
        }

        #endregion


        #region // Other methods //

        protected static string GetMonth(int monthNo)
        {
            string[] months = { "January", "February", "March", "April", "May", "June", "July", "Auguest", "September", "October", "November", "December" };

            if (monthNo >= 1 && monthNo <= 12)
            {
                return months[monthNo - 1];
            }

            // Handle invalid month numbers
            return "InvalidMonth";
        }

        protected static int? GetProgressBar(decimal amount, decimal totalAmt)
        {
            var progessBar = (int)(amount / totalAmt * 100);
            return progessBar;
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
