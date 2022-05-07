using FBook.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FBook.Areas.Identity.Data;

// Add profile data for application users by adding properties to the FBookUser class
public class FBookUser : IdentityUser
{

    [DataType(DataType.DateTime)]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddThh:mm:ss}")]
    [Display(Name = "Registed Date")]
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public Store? Store { get; set; }
    public virtual ICollection<Order>? Orders { get; set; }
    public virtual ICollection<Cart>? Carts { get; set; }
}

