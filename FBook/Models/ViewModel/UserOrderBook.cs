using FBook.Areas.Identity.Data;

namespace FBook.ViewModel
{
    public class UserOrderBook
    {
        public FBookUser? User { get; set; }
        public Book? Book { get; set; }
        public IEnumerable<Order> OrderList { get; set; }
    }
}