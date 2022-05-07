using FBook.Areas.Identity.Data;
using FBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FBook.Data;

public class FBookContext : IdentityDbContext<FBookUser>
{
    public FBookContext(DbContextOptions<FBookContext> options)
        : base(options)
    {
    }
    public DbSet<Store> Store { get; set; }
    public DbSet<Book> Book { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<OrderDetail> OrderDetail { get; set; }
    public DbSet<Cart> Cart { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<FBookUser>()
            .HasOne<Store>(au => au.Store)
            .WithOne(st => st.User)
            .HasForeignKey<Store>(st => st.UId);

        builder.Entity<Book>()
            .HasOne<Store>(b => b.Store)
            .WithMany(st => st.Books)
            .HasForeignKey(b => b.StoreId);

        builder.Entity<Order>()
            .HasOne<FBookUser>(o => o.User)
            .WithMany(ap => ap.Orders)
            .HasForeignKey(o => o.UId);

        builder.Entity<OrderDetail>()
            .HasKey(od => new { od.OrderId, od.BookIsbn });
        builder.Entity<OrderDetail>()
            .HasOne<Order>(od => od.Order)
            .WithMany(or => or.OrderDetails)
            .HasForeignKey(od => od.OrderId);
        builder.Entity<OrderDetail>()
            .HasOne<Book>(od => od.Book)
            .WithMany(b => b.OrderDetails)
            .HasForeignKey(od => od.BookIsbn);
        builder.Entity<Cart>()
            .HasKey(c => new { c.UId, c.BookIsbn });
        builder.Entity<Cart>()
            .HasOne<FBookUser>(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UId);
        builder.Entity<Cart>()
            .HasOne<Book>(od => od.Book)
            .WithMany(b => b.Carts)
            .HasForeignKey(od => od.BookIsbn)
            .OnDelete(DeleteBehavior.NoAction);


    }
}
