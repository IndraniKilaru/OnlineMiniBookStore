using Microsoft.EntityFrameworkCore;
using BookStore.Shared.Models;

namespace BookStore.BooksService.Data
{
    public class BooksDbContext : DbContext
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Author).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ISBN).HasMaxLength(20);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Category).HasMaxLength(100);
            });            // Seed some books
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ISBN = "9780743273565",
                    Price = 12.99m,
                    Description = "A classic American novel about the Jazz Age",
                    Category = "Fiction",
                    StockQuantity = 50,
                    ImageUrl = "https://covers.openlibrary.org/b/isbn/9780743273565-M.jpg",
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Book
                {
                    Id = 2,
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    ISBN = "9780061120084",
                    Price = 14.99m,
                    Description = "A gripping tale of racial injustice and childhood innocence",
                    Category = "Fiction",
                    StockQuantity = 30,
                    ImageUrl = "https://covers.openlibrary.org/b/isbn/9780061120084-M.jpg",
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Book
                {
                    Id = 3,
                    Title = "1984",
                    Author = "George Orwell",
                    ISBN = "9780451524935",
                    Price = 13.99m,
                    Description = "A dystopian social science fiction novel",
                    Category = "Science Fiction",
                    StockQuantity = 40,
                    ImageUrl = "https://covers.openlibrary.org/b/isbn/9780451524935-M.jpg",
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Book
                {
                    Id = 4,
                    Title = "Pride and Prejudice",
                    Author = "Jane Austen",
                    ISBN = "9780141439518",
                    Price = 11.99m,
                    Description = "A romantic novel set in Georgian England",
                    Category = "Romance",
                    StockQuantity = 25,
                    ImageUrl = "https://covers.openlibrary.org/b/isbn/9780141439518-M.jpg",
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                new Book
                {
                    Id = 5,
                    Title = "The Catcher in the Rye",
                    Author = "J.D. Salinger",
                    ISBN = "9780316769174",
                    Price = 13.50m,
                    Description = "A coming-of-age story in 1950s New York",
                    Category = "Fiction",
                    StockQuantity = 35,
                    ImageUrl = "https://covers.openlibrary.org/b/isbn/9780316769174-M.jpg",
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );
        }
    }
}
