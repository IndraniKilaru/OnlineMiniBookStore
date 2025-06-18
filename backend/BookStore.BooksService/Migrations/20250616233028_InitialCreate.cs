using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStore.BooksService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "Category", "CreatedAt", "Description", "ISBN", "ImageUrl", "Price", "StockQuantity", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "F. Scott Fitzgerald", "Fiction", new DateTime(2025, 6, 16, 23, 30, 25, 254, DateTimeKind.Utc).AddTicks(112), "A classic American novel about the Jazz Age", "9780743273565", "https://covers.openlibrary.org/b/id/8225261-L.jpg", 12.99m, 50, "The Great Gatsby", new DateTime(2025, 6, 16, 23, 30, 25, 254, DateTimeKind.Utc).AddTicks(200) },
                    { 2, "Harper Lee", "Fiction", new DateTime(2025, 6, 16, 23, 30, 25, 254, DateTimeKind.Utc).AddTicks(276), "A gripping tale of racial injustice and childhood innocence", "9780061120084", "https://covers.openlibrary.org/b/id/8225261-L.jpg", 14.99m, 30, "To Kill a Mockingbird", new DateTime(2025, 6, 16, 23, 30, 25, 254, DateTimeKind.Utc).AddTicks(277) },
                    { 3, "George Orwell", "Science Fiction", new DateTime(2025, 6, 16, 23, 30, 25, 254, DateTimeKind.Utc).AddTicks(279), "A dystopian social science fiction novel", "9780451524935", "https://covers.openlibrary.org/b/id/8225261-L.jpg", 13.99m, 40, "1984", new DateTime(2025, 6, 16, 23, 30, 25, 254, DateTimeKind.Utc).AddTicks(280) },
                    { 4, "Jane Austen", "Romance", new DateTime(2025, 6, 16, 23, 30, 25, 254, DateTimeKind.Utc).AddTicks(282), "A romantic novel set in Georgian England", "9780141439518", "https://covers.openlibrary.org/b/id/8225261-L.jpg", 11.99m, 25, "Pride and Prejudice", new DateTime(2025, 6, 16, 23, 30, 25, 254, DateTimeKind.Utc).AddTicks(282) },
                    { 5, "J.D. Salinger", "Fiction", new DateTime(2025, 6, 16, 23, 30, 25, 254, DateTimeKind.Utc).AddTicks(284), "A coming-of-age story in 1950s New York", "9780316769174", "https://covers.openlibrary.org/b/id/8225261-L.jpg", 13.50m, 35, "The Catcher in the Rye", new DateTime(2025, 6, 16, 23, 30, 25, 254, DateTimeKind.Utc).AddTicks(284) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
