using FBook.Areas.Identity.Data;
using FBook.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBook.Controllers
{
    public class StoreController : Controller
    {
        private readonly FBookContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<FBookUser> _userManager;


        public StoreController(FBookContext db, IWebHostEnvironment hostEnvironment, UserManager<FBookUser> userManager)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string SearchString)
        {
            ViewData["CurrentFilter"] = SearchString;
            var books = from b in _db.Book
                        select b;
            if (!String.IsNullOrEmpty(SearchString))
            {
                books = books.Where(b => b.Title.Contains(SearchString));
            }
            /*if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            ViewBag.CurrentFilter = SearchString;
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(books.ToPagedList(pageNumber, pageSize));*/
            return View(books);

        }
    }
}
