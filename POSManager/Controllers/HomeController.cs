using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSManager.Data;
using POSManager.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace POSManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly POSManagerDbContext _context;

        public HomeController(ILogger<HomeController> logger, POSManagerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        #region // Main methods //

        public IActionResult Index()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity != null && claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "SaleDashboard");
            }
            return RedirectToAction("LogIn", "Home"); // Use RedirectToAction instead of RedirectToActionResult
        }

        #endregion


        #region // Login methods //

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AjaxLogin(string uc, string p)
        {
            if ((!string.IsNullOrEmpty(uc)) && (!string.IsNullOrEmpty(p)))
            {
                try
                {
                    var logInUser = await _context.ms_companyuser.Where(x => x.LoginNme == uc && x.ActiveFlg == true).Select(x => x).FirstOrDefaultAsync();

                    if (logInUser != null)
                    {
                        if (logInUser.Pwd == p)
                        {
                            try
                            {
                                var claims = new List<Claim>() {
                                new (ClaimTypes.NameIdentifier, logInUser.LoginNme)
                            };

                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                var properties = new AuthenticationProperties()
                                {
                                    AllowRefresh = true
                                };

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                                var cmpy = await _context.ms_company
                                .Where(x => x.LoginNme == logInUser.LoginNme)
                                .Select(x => x.CmpyNme)
                                .FirstOrDefaultAsync() ?? "";

                                return Json(cmpy);

                            }
                            catch (Exception ex)
                            {
                                return Json(ex.Message);
                            }
                        }
                        return Json("#Error.Password is incorrect!");
                    }
                    return Json("#Error.Usercode is incorrect!");
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }
            }
            return Json("#Error.User code or Password should not be empty!");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }

        #endregion


        #region // Unnecessary methods //

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion


        #region // Global methods (Important)//

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
