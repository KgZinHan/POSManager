using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSManager.Data;
using POSManager.Models;

namespace POSManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DateCheckAPIController : ControllerBase
    {
        private readonly POSManagerDbContext _context;
        public DateCheckAPIController(POSManagerDbContext context)
        {
            _context = context;
        }

        #region // Main methods //

        // GET api/DateCheckAPI
        [HttpGet]
        public async Task<List<DateTime>> Get([FromQuery] int month,[FromQuery] int year)
        {
            var missingDate = new List<DateTime>();

            var startDate = new DateTime(year, month, 1);

            var endDate = startDate.AddMonths(1).AddDays(-1);

            for (var date = startDate; date <= endDate;date = date.AddDays(1))
            {
                if (!await _context.pmgr_bill.AnyAsync(b => b.Bizdte.Date == date))
                {
                    missingDate.Add(date);
                }
            }

            return missingDate;
        }

        #endregion
    }
}
