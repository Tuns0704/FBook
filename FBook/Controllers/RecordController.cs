using FBook.Areas.Identity.Data;
using FBook.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBook.Controllers
{
    public class RecordController : Controller
    {
        private readonly FBookContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<FBookUser> _userManager;


        public RecordController(FBookContext db, IWebHostEnvironment hostEnvironment, UserManager<FBookUser> userManager)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Order.ToListAsync());
        }
        public async Task<IActionResult> Details(int orderid)
        {

            return View(await _db.OrderDetail.FirstOrDefaultAsync(b => b.OrderId == orderid));
        }
    }
}
