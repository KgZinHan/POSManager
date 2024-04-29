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
    public class YearlySalesController : Controller
    {
        private readonly POSManagerDbContext _context;

        public YearlySalesController(POSManagerDbContext context)
        {
            _context = context;
        }

        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            var loginName = GetLoginName();

            var yearlySales = new YearlySales()
            {
                Year = DateTime.Now.Year
            };

            ViewData["BranchNames"] = new SelectList(_context.ms_company.Where(cmpy => cmpy.LoginNme == loginName), "CmpyNme", "CmpyNme");

            return View(yearlySales);
        }

        [HttpPost]
        public IActionResult View(string branchName, int year, int exYear)
        {
            var loginName = GetLoginName();
            var cmpygrpnme = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyGrpNme).FirstOrDefault();
            var yearlySalesAction = 0;

            var mainList = new List<YearlySalesHeadModel>();

            var yearlySales = _context.spYearlySalesDbA0Set
                    .FromSqlRaw("EXEC sp_yearlySales @action={0}, @cmpygrpnme = {1}, @year = {2},@exyear = {3}", yearlySalesAction, cmpygrpnme, year, exYear)
                    .AsEnumerable()
                    .Select(x => new YearlySalesModel
                    {
                        Branch = x.Branch,
                        YearNo = x.YearNo,
                        Amount = x.Amount
                    })
                    .ToList();

            if (!branchName.IsNullOrEmpty()) // branchname is there
            {
                var newYearlySales = yearlySales.Where(list => list.Branch == branchName).ToList();

                var totalAmt = newYearlySales.Sum(list => list.Amount);

                foreach (var item in newYearlySales)
                {
                    item.No = newYearlySales.IndexOf(item) + 1;
                    item.ProgressBar = GetProgressBar(item.Amount ?? 0, totalAmt ?? 0);
                }

                var headModel = new YearlySalesHeadModel()
                {
                    YearlySales = newYearlySales,
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
                    var newYearlySales = yearlySales.Where(x => x.Branch == branch).ToList();
                    var totalAmt = newYearlySales.Sum(list => list.Amount);

                    foreach (var item in newYearlySales)
                    {
                        item.No = newYearlySales.IndexOf(item) + 1;
                        item.ProgressBar = GetProgressBar(item.Amount ?? 0, totalAmt ?? 0);
                    }

                    var headModel = new YearlySalesHeadModel()
                    {
                        YearlySales = newYearlySales,
                        Branch = branch,
                        TotalAmount = totalAmt
                    };
                    mainList.Add(headModel);
                }
            }

            return PartialView("_YearlySalesViewPartial", mainList);
        }

        public IActionResult GetDonutChartData(string branchName, int year, int exYear)
        {
            var loginName = GetLoginName();
            var cmpygrpnme = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyGrpNme).FirstOrDefault();
            var yearlySalesAction = 0;

            if (branchName.IsNullOrEmpty()) // for all branches
            {

                var allBranches = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyNme).ToList();

                // Add chartData labels
                var newLabels = new List<string>();
                newLabels.AddRange(allBranches);

                // Add chartData datasets data
                var yearlySales = _context.spYearlySalesDbA0Set
                    .FromSqlRaw("EXEC sp_yearlySales @action={0}, @cmpygrpnme = {1}, @year = {2},@exyear = {3}", yearlySalesAction, cmpygrpnme, year, exYear)
                    .AsEnumerable()
                    .Select(x => new YearlySalesModel
                    {
                        Branch = x.Branch,
                        Amount = x.Amount
                    })
                    .ToList();

                var newData = new List<int>();

                foreach (var branch in allBranches)
                {
                    var totalAmt = (int)(yearlySales.Where(x => x.Branch == branch).Sum(x => x.Amount) ?? 0);
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
                var yearlySales = _context.spYearlySalesDbA0Set
                    .FromSqlRaw("EXEC sp_yearlySales @action={0}, @cmpygrpnme = {1}, @year = {2},@exyear = {3}", yearlySalesAction, cmpygrpnme, year, exYear)
                    .AsEnumerable()
                    .Select(x => new YearlySalesModel
                    {
                        Branch = x.Branch,
                        Amount = x.Amount
                    })
                    .Where(x => x.Branch == branchName)
                    .ToList();

                var totalAmt = (int)(yearlySales.Sum(x => x.Amount) ?? 0);
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

        public IActionResult GetBarChartData(string branchName, int year, int exYear)
        {
            var loginName = GetLoginName();
            var cmpygrpnme = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyGrpNme).FirstOrDefault();

            // both null or empty
            var catgSalesAction = 1;
            var categories = new List<string>();

            if (branchName.IsNullOrEmpty())
            {
                categories = _context.pmgr_bill
                  .GroupBy(bill => bill.CatgCde)
                  .Select(group => group.Key)
                  .Distinct()
                  .ToList();
            }
            else
            {
                catgSalesAction = 2;
                categories = _context.pmgr_bill
                   .Where(bill => bill.CmpyNme == branchName)
                   .GroupBy(bill => bill.CatgCde)
                   .Select(group => group.Key)
                   .Distinct()
                   .ToList();
            }

            // Add chartData labels
            var newLabels = new List<string>();
            newLabels.AddRange(categories);

            // Add chartData datasets data
            var catgWiseSales = _context.spYearlySalesDbA1Set
                .FromSqlRaw("EXEC sp_yearlySales @action={0}, @cmpygrpnme = {1}, @year = {2},@exyear = {3},@cmpynme = {4}", catgSalesAction, cmpygrpnme, year, exYear, branchName)
                .AsEnumerable()
                .Select(x => new YearlyCatgWiseSalesModel
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
