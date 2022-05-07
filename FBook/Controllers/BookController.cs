using FBook.Areas.Identity.Data;
using FBook.Data;
using FBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBook.Controllers
{
    [Authorize(Roles = "Seller")]
    public class BookController : Controller
    {
        private readonly FBookContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<FBookUser> _userManager;
        private readonly int _recordsPerPage = 5;


        public BookController(FBookContext db, IWebHostEnvironment hostEnvironment, UserManager<FBookUser> userManager)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
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

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Isbn,Title,Pages,Author,Category,Price,Desc")] Book book, IFormFile image)
        {
            if (image != null)
            {
                string imgName = book.Isbn + Path.GetExtension(image.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imgName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                book.ImgUrl = "images/" + imgName;

                var thisUserId = _userManager.GetUserId(HttpContext.User);
                Store thisStore = await _db.Store.FirstOrDefaultAsync(s => s.UId == thisUserId);
                book.StoreId = thisStore.Id;
            }
            else
            {
                return View(book);
            }
            _db.Add(book);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET
        public async Task<IActionResult> Edit(string? Isbn)
        {
            if (Isbn == null)
            {
                return NotFound();
            }
            var bookFromDb = await _db.Book.FindAsync(Isbn);
            if (bookFromDb == null)
            {
                return NotFound();
            }
            return View(bookFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Isbn,Title,Pages,Author,Category,Price,Desc")] Book book)
        {
            if (ModelState.IsValid)
            {
                _db.Book.Update(book);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            TempData["success"] = "Update Book success";
            return View(book);
        }

        //GET
        public async Task<IActionResult> Delete(string? Isbn)
        {
            if (Isbn == null)
            {
                return NotFound();
            }
            var bookFromDb = await _db.Book.FindAsync(Isbn);
            if (bookFromDb == null)
            {
                return NotFound();
            }
            return View(bookFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(string? Isbn)
        {
            var book = _db.Book.Find(Isbn);
            if (book == null)
            {
                return NotFound();
            }
            _db.Book.Remove(book);
            await _db.SaveChangesAsync();
            TempData["success"] = "Delete Book success";
            return RedirectToAction("Index");
        }
    }
}
