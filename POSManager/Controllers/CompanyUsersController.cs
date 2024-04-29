using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSManager.Data;
using POSManager.Models;

namespace POSManager.Controllers
{
    [Authorize]
    public class CompanyUsersController : Controller
    {
        private readonly POSManagerDbContext _context;

        public CompanyUsersController(POSManagerDbContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public async Task<IActionResult> Index()
        {
            SetLayOutData();

            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            var companyUserList = await _context.ms_companyuser.ToListAsync();

            return View(companyUserList);
        }

        public IActionResult Create()
        {
            SetLayOutData();

            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoginNme,Pwd,ActiveFlg")] CompanyUser companyUser)
        {
            SetLayOutData();

            if (CompanyUserExists(companyUser.LoginNme))
            {
                ModelState.AddModelError("LoginNme", "Login with this name already exists.");
                return View(companyUser);
            }

            if (ModelState.IsValid)
            {
                companyUser.CreateDteTime = DateTime.Now;
                _context.Add(companyUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(companyUser);
        }

        public async Task<IActionResult> Edit(string id)
        {
            SetLayOutData();

            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || _context.ms_companyuser == null)
            {
                return NotFound();
            }

            var companyUser = await _context.ms_companyuser.FindAsync(id);
            if (companyUser == null)
            {
                return NotFound();
            }

            return View(companyUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("LoginNme,Pwd,ActiveFlg,CreateDteTime")] CompanyUser companyUser)
        {
            SetLayOutData();

            if (id != companyUser.LoginNme)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyUserExists(companyUser.LoginNme))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(companyUser);
        }

        public async Task<IActionResult> Delete(string id)
        {
            SetLayOutData();

            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || _context.ms_companyuser == null)
            {
                return NotFound();
            }

            var companyUser = await _context.ms_companyuser
                .FirstOrDefaultAsync(m => m.LoginNme == id);
            if (companyUser == null)
            {
                return NotFound();
            }

            return View(companyUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            SetLayOutData();

            if (_context.ms_companyuser == null)
            {
                return Problem("Entity set 'POSManagerDbContext.ms_companyuser'  is null.");
            }
            var companyUser = await _context.ms_companyuser.FindAsync(id);
            if (companyUser != null)
            {
                _context.ms_companyuser.Remove(companyUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyUserExists(string id)
        {
            return (_context.ms_companyuser?.Any(e => e.LoginNme == id)).GetValueOrDefault();
        }

        #endregion


        #region // Global methods (Important)//

        protected string GetLoginName()
        {
            var loginName = HttpContext.User.Claims.FirstOrDefault()?.Value;
            return loginName ?? "";
        }

        protected bool CheckAdmin()
        {
            var flag = false;

            var loginName = HttpContext.User.Claims.FirstOrDefault()?.Value;
            if (loginName?.ToUpper() == "ADMIN")
            {
                flag = true;
            }

            return flag;
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
