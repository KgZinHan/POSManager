using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POSManager.Data;
using POSManager.Models;

namespace POSManager.Controllers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly POSManagerDbContext _context;

        public CompaniesController(POSManagerDbContext context)
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

            var companyList = await _context.ms_company.ToListAsync();

            ViewData["LoginNames"] = new SelectList(_context.ms_companyuser, "LoginNme", "LoginNme");

            return View(companyList);
        }

        public IActionResult Create()
        {
            SetLayOutData();

            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["LoginNames"] = new SelectList(_context.ms_companyuser, "LoginNme", "LoginNme");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CmpyNme,CmpyGrpNme,LoginNme,Address")] Company company)
        {
            SetLayOutData();

            if (CompanyExists(company.CmpyNme))
            {
                ModelState.AddModelError("CmpyNme", "Company with this name already exists.");
                ViewData["LoginNames"] = new SelectList(_context.ms_companyuser, "LoginNme", "LoginNme");
                return View(company);
            }

            if (ModelState.IsValid)
            {
                company.CreateDteTime = DateTime.Now;
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["LoginNames"] = new SelectList(_context.ms_companyuser, "LoginNme", "LoginNme");
            return View(company);
        }

        public async Task<IActionResult> Edit(string id)
        {
            SetLayOutData();

            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || _context.ms_company == null)
            {
                return NotFound();
            }

            var company = await _context.ms_company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            ViewData["LoginNames"] = new SelectList(_context.ms_companyuser, "LoginNme", "LoginNme");

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CmpyNme,CmpyGrpNme,LoginNme,Address,CreateDteTime")] Company company)
        {
            SetLayOutData();

            if (id != company.CmpyNme)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.CmpyNme))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                ViewData["LoginNames"] = new SelectList(_context.ms_companyuser, "LoginNme", "LoginNme");
                return RedirectToAction(nameof(Index));
            }

            ViewData["LoginNames"] = new SelectList(_context.ms_companyuser, "LoginNme", "LoginNme");
            return View(company);
        }

        public async Task<IActionResult> Delete(string id)
        {
            SetLayOutData();

            if (!CheckAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || _context.ms_company == null)
            {
                return NotFound();
            }

            var company = await _context.ms_company
                .FirstOrDefaultAsync(m => m.CmpyNme == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            SetLayOutData();

            if (_context.ms_company == null)
            {
                return Problem("Entity set 'POSManagerDbContext.ms_company'  is null.");
            }
            var company = await _context.ms_company.FindAsync(id);
            if (company != null)
            {
                _context.ms_company.Remove(company);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(string id)
        {
            return (_context.ms_company?.Any(e => e.CmpyNme == id)).GetValueOrDefault();
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
