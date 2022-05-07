using FBook.Areas.Identity.Data;
using FBook.Data;
using FBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<FBookUser> _userManager;
        private readonly FBookContext _db;
        private readonly int _recordsPerPage = 20;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender, UserManager<FBookUser> userManager, FBookContext db)
        {
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;
            _db = db;

        }

        [Authorize(Roles = "Customer")]
        public IActionResult ForCustomerOnly()
        {
            ViewBag.message = "This is for Customer only! Hi " + _userManager.GetUserName(HttpContext.User);
            return View("Views/Home/Index.cshtml");
        }

        [Authorize(Roles = "Seller")]
        public IActionResult ForSellerOnly()
        {
            ViewBag.message = "This is for Store Owner only!";
            return View("Views/Home/Index.cshtml");
        }



        public async Task<IActionResult> Index(string searchString, int id = 0)
        {
            ViewBag.CurrentFilter = searchString;
            var query = from s in _db.Book
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Title.Contains(searchString));
            }

            int numberOfRecords = await _db.Book.CountAsync();     //Count SQL
            int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / _recordsPerPage);
            ViewBag.numberOfPages = numberOfPages;
            ViewBag.currentPage = id;
            List<Book> books = await query
                .Skip(id * _recordsPerPage)  //Offset SQL
                .Take(_recordsPerPage)       //Top SQL
                .ToListAsync();

            return View(books);
        }

        public async Task<IActionResult> Details(string Isbn)
        {

            return View(await _db.Book.FirstOrDefaultAsync(b => b.Isbn == Isbn));
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Help()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}