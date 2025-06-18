using Microsoft.EntityFrameworkCore;
using BookStore.Shared.Models;
using BookStore.Shared.DTOs;
using BookStore.CartService.Data;

namespace BookStore.CartService.Services
{
    public interface ICartService
    {
        Task<CartSummaryDto> GetCartAsync(int userId);
        Task<CartItemDto> AddToCartAsync(int userId, AddToCartDto addToCartDto);
        Task<CartItemDto> UpdateCartItemAsync(int userId, int itemId, UpdateCartItemDto updateDto);
        Task<bool> RemoveFromCartAsync(int userId, int itemId);
        Task<bool> ClearCartAsync(int userId);
    }

    public class CartServiceImpl : ICartService
    {
        private readonly CartDbContext _context;

        public CartServiceImpl(CartDbContext context)
        {
            _context = context;
        }        public async Task<CartSummaryDto> GetCartAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    BookId = ci.BookId,
                    Quantity = ci.Quantity,
                    Book = GetMockBookDetails(ci.BookId)
                })
                .ToListAsync();

            var summary = new CartSummaryDto
            {
                Items = cartItems,
                TotalItems = cartItems.Sum(i => i.Quantity),
                TotalAmount = cartItems.Sum(i => i.Quantity * (i.Book?.Price ?? 0))
            };

            return summary;
        }

        public async Task<CartItemDto> AddToCartAsync(int userId, AddToCartDto addToCartDto)
        {
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.BookId == addToCartDto.BookId);

            if (existingItem != null)
            {
                existingItem.Quantity += addToCartDto.Quantity;
                _context.CartItems.Update(existingItem);
            }
            else
            {
                existingItem = new CartItem
                {
                    UserId = userId,
                    BookId = addToCartDto.BookId,
                    Quantity = addToCartDto.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                _context.CartItems.Add(existingItem);
            }

            await _context.SaveChangesAsync();            return new CartItemDto
            {
                Id = existingItem.Id,
                BookId = existingItem.BookId,
                Quantity = existingItem.Quantity,
                Book = GetMockBookDetails(existingItem.BookId)
            };
        }

        public async Task<CartItemDto> UpdateCartItemAsync(int userId, int itemId, UpdateCartItemDto updateDto)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.UserId == userId);

            if (cartItem == null)
                throw new InvalidOperationException("Cart item not found");

            cartItem.Quantity = updateDto.Quantity;
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();            return new CartItemDto
            {
                Id = cartItem.Id,
                BookId = cartItem.BookId,
                Quantity = cartItem.Quantity,
                Book = GetMockBookDetails(cartItem.BookId)
            };
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int itemId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.UserId == userId);

            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }        public async Task<bool> ClearCartAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }        private BookDto GetMockBookDetails(int bookId)
        {
            // Mock book data - in real app would call BooksService
            var books = new Dictionary<int, BookDto>
            {
                { 1, new BookDto { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Price = 12.99m, ImageUrl = "https://via.placeholder.com/150x200/4A90E2/FFFFFF?text=The+Great+Gatsby" } },
                { 2, new BookDto { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", Price = 14.99m, ImageUrl = "https://via.placeholder.com/150x200/E94B3C/FFFFFF?text=To+Kill+a+Mockingbird" } },
                { 3, new BookDto { Id = 3, Title = "1984", Author = "George Orwell", Price = 13.99m, ImageUrl = "https://via.placeholder.com/150x200/50C878/FFFFFF?text=1984" } },
                { 4, new BookDto { Id = 4, Title = "Pride and Prejudice", Author = "Jane Austen", Price = 11.99m, ImageUrl = "https://via.placeholder.com/150x200/FFB347/FFFFFF?text=Pride+and+Prejudice" } },
                { 5, new BookDto { Id = 5, Title = "The Catcher in the Rye", Author = "J.D. Salinger", Price = 13.50m, ImageUrl = "https://via.placeholder.com/150x200/8E44AD/FFFFFF?text=The+Catcher+in+the+Rye" } }
            };

            return books.ContainsKey(bookId) ? books[bookId] : 
                new BookDto { Id = bookId, Title = $"Book {bookId}", Author = "Unknown Author", Price = 15.99m, ImageUrl = "https://via.placeholder.com/150x200/95A5A6/FFFFFF?text=Book+Cover" };
        }
    }
}
