using Microsoft.EntityFrameworkCore;
using BookStore.Shared.Models;

namespace BookStore.CartService.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
        {
        }

        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.BookId).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                
                // Create index for faster querying by UserId
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.UserId, e.BookId }).IsUnique();
            });
        }
    }
}
