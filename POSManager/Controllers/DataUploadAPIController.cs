using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSManager.Data;
using POSManager.Models;

namespace POSManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataUploadAPIController : ControllerBase
    {
        private readonly POSManagerDbContext _context;

        public DataUploadAPIController(POSManagerDbContext context)
        {
            _context = context;
        }

        #region // Main methods //

        // GET: api/DataUploadAPI
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/DataUploadAPI
        [HttpPost]
        public async Task Post([FromBody] UploadAllData model)
        {

            var billList = model.BillList;
            var billPList = model.BillPList;

            var bizDte = billList.Select(b => b.Bizdte).FirstOrDefault();

            // Delete previous data first if there is data
            _context.pmgr_bill.Where(b => b.Bizdte.Date == bizDte.Date).ExecuteDelete();
            _context.pmgr_billp.Where(b => b.BizDte.Date == bizDte.Date).ExecuteDelete();

            // Add new data 
            foreach (var bill in billList)
            {
                await _context.pmgr_bill.AddAsync(bill);
            }

            foreach (var billP in billPList)
            {
                await _context.pmgr_billp.AddAsync(billP);
            }

            await _context.SaveChangesAsync();
        }

        #endregion

    }
}
