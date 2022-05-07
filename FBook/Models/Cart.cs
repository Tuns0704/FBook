using FBook.Areas.Identity.Data;

namespace FBook.Models
{
    public class Cart
    {
        public string UId { get; set; }
        public string BookIsbn { get; set; }
        public FBookUser? User { get; set; }
        public Book? Book { get; set; }

    }
}
