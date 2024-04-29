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
    public class SalesAnalysisController : Controller
    {
        private readonly POSManagerDbContext _context;

        public SalesAnalysisController(POSManagerDbContext context)
        {
            _context = context;
        }

        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            var loginName = GetLoginName();

            var saleAnalysis = new SalesAnalysis()
            {
                FromDate = DateTime.Now,
                NextDay = 1,
                TopBottom = 1
            };

            ViewData["BranchNames"] = new SelectList(_context.ms_company.Where(cmpy => cmpy.LoginNme == loginName), "CmpyNme", "CmpyNme");

            return View(saleAnalysis);
        }

        [HttpPost]
        public IActionResult Search(string branchName, DateTime fromDate, int days, string mode, int topBottom)
        {
            var loginName = GetLoginName();
            var cmpygrpnme = _context.ms_company.Where(cmpy => cmpy.LoginNme == loginName).Select(cmpy => cmpy.CmpyGrpNme).FirstOrDefault();
            var saleAnalysisAction = 0;

            if (branchName.IsNullOrEmpty() && mode == "Top")
            {
                saleAnalysisAction = 0;
            }
            if (!branchName.IsNullOrEmpty() && mode == "Top")
            {
                saleAnalysisAction = 1;
            }
            if (branchName.IsNullOrEmpty() && mode == "Bottom")
            {
                saleAnalysisAction = 2;
            }
            if (!branchName.IsNullOrEmpty() && mode == "Bottom")
            {
                saleAnalysisAction = 3;
            }

            try
            {
                var saleAnalysisList = _context.spSaleAnalysisDbA0Set
                        .FromSqlRaw("EXEC sp_saleAnalysis @action={0}, @cmpygrpnme = {1},@number = {2},@bizdte = {3},@days = {4},@cmpynme = {5} ", saleAnalysisAction, cmpygrpnme, topBottom, fromDate.Date, days, branchName)
                        .AsEnumerable()
                        .Select(x => new SaleAnalysisDbModel
                        {
                            ItemDesc = x.ItemDesc,
                            SaleAmount = x.SaleAmount,
                            Qty = x.Qty
                        })
                        .ToList();

                foreach (var item in saleAnalysisList)
                {
                    item.No = saleAnalysisList.IndexOf(item) + 1;
                }
                return PartialView("_SaleAnalysisSearchPartial", saleAnalysisList);
            }
            catch (Exception Ex)
            {
                return RedirectToAction("Index");
            }
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
