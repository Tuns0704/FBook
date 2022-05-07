using FBook.Areas.Identity.Data;
using FBook.Data;
using FBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBook.Controllers
{
    public class CartController : Controller
    {
        private readonly FBookContext _db;
        private readonly UserManager<FBookUser> _userManager;

        public CartController(FBookContext context, UserManager<FBookUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            return View(_db.Cart.Where(c => c.UId == thisUserId));
        }

        public async Task<IActionResult> AddToCart(string Isbn)
        {

            try
            {
                string thisUserId = _userManager.GetUserId(HttpContext.User);
                Cart myCart = new Cart() { UId = thisUserId, BookIsbn = Isbn };
                Cart fromDb = _db.Cart.FirstOrDefault(c => c.UId == thisUserId && c.BookIsbn == Isbn);
                if (fromDb == null)
                {
                    _db.Add(myCart);
                    await _db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
        }
        public async Task<IActionResult> Checkout()
        {
            return View();
        }

        public async Task<IActionResult> Order()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            List<Cart> myDetailsInCart = await _db.Cart
                .Where(c => c.UId == thisUserId)
                .Include(c => c.Book)
                .ToListAsync();
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    //Step 1: create an order
                    Order myOrder = new Order();
                    myOrder.UId = thisUserId;
                    myOrder.OrderDate = DateTime.Now;
                    myOrder.Total = myDetailsInCart.Select(c => c.Book.Price)
                        .Aggregate((c1, c2) => c1 + c2);
                    _db.Add(myOrder);
                    await _db.SaveChangesAsync();

                    //Step 2: insert all order details by var "myDetailsInCart"
                    foreach (var item in myDetailsInCart)
                    {
                        OrderDetail detail = new OrderDetail()
                        {
                            OrderId = myOrder.Id,
                            BookIsbn = item.BookIsbn,
                            Quantity = 1
                        };
                        _db.Add(detail);
                    }
                    await _db.SaveChangesAsync();

                    //Step 3: empty/delete the cart we just done for thisUser
                    _db.Cart.RemoveRange(myDetailsInCart);
                    await _db.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error occurred in Checkout" + ex);
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
