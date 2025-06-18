using Microsoft.EntityFrameworkCore;
using BookStore.BooksService.Data;
using BookStore.Shared.Models;
using BookStore.Shared.DTOs;
using BookStore.Shared.Common;
using BookStore.Shared.Services;

namespace BookStore.BooksService.Services
{
    public class BooksService : IBooksService
    {
        private readonly BooksDbContext _context;
        private readonly ICacheService _cacheService;
        private const int CacheExpirationMinutes = 30;

        public BooksService(BooksDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }        public async Task<PagedResult<BookDto>> GetBooksAsync(int page = 1, int pageSize = 10, string? category = null, string? searchTerm = null)
        {
            // Create cache key based on parameters
            var cacheKey = $"books:page-{page}:size-{pageSize}:category-{category ?? "all"}:search-{searchTerm ?? "all"}";
            
            // Try to get from cache first
            var cachedResult = await _cacheService.GetAsync<PagedResult<BookDto>>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(b => b.Category.ToLower().Contains(category.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => b.Title.ToLower().Contains(searchTerm.ToLower()) ||
                                        b.Author.ToLower().Contains(searchTerm.ToLower()) ||
                                        b.Description.ToLower().Contains(searchTerm.ToLower()));
            }

            var totalCount = await query.CountAsync();
            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => MapToDto(b))
                .ToListAsync();

            var result = new PagedResult<BookDto>
            {
                Items = books,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };            // Cache the result for 30 minutes
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CacheExpirationMinutes));
            
            return result;
        }        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var cacheKey = $"book:{id}";
            
            // Try to get from cache first
            var cachedBook = await _cacheService.GetAsync<BookDto>(cacheKey);
            if (cachedBook != null)
            {
                return cachedBook;
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            var bookDto = MapToDto(book);
            
            // Cache the book for 60 minutes
            await _cacheService.SetAsync(cacheKey, bookDto, TimeSpan.FromMinutes(60));
            
            return bookDto;
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto bookDto)
        {
            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                ISBN = bookDto.ISBN,
                Price = bookDto.Price,
                Description = bookDto.Description,
                Category = bookDto.Category,
                StockQuantity = bookDto.StockQuantity,
                ImageUrl = bookDto.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return MapToDto(book);
        }

        public async Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto bookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            if (!string.IsNullOrEmpty(bookDto.Title))
                book.Title = bookDto.Title;
            if (!string.IsNullOrEmpty(bookDto.Author))
                book.Author = bookDto.Author;
            if (!string.IsNullOrEmpty(bookDto.ISBN))
                book.ISBN = bookDto.ISBN;
            if (bookDto.Price.HasValue)
                book.Price = bookDto.Price.Value;
            if (!string.IsNullOrEmpty(bookDto.Description))
                book.Description = bookDto.Description;
            if (!string.IsNullOrEmpty(bookDto.Category))
                book.Category = bookDto.Category;
            if (bookDto.StockQuantity.HasValue)
                book.StockQuantity = bookDto.StockQuantity.Value;
            if (!string.IsNullOrEmpty(bookDto.ImageUrl))
                book.ImageUrl = bookDto.ImageUrl;

            book.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(book);
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _context.Books
                .Select(b => b.Category)
                .Distinct()
                .Where(c => !string.IsNullOrEmpty(c))
                .ToListAsync();
        }

        public async Task<bool> UpdateImageUrlsAsync()
        {
            var books = await _context.Books.ToListAsync();
            
            foreach (var book in books)
            {
                book.ImageUrl = book.Id switch
                {
                    1 => "https://covers.openlibrary.org/b/isbn/9780743273565-M.jpg",
                    2 => "https://covers.openlibrary.org/b/isbn/9780061120084-M.jpg", 
                    3 => "https://covers.openlibrary.org/b/isbn/9780451524935-M.jpg",
                    4 => "https://covers.openlibrary.org/b/isbn/9780141439518-M.jpg",
                    5 => "https://covers.openlibrary.org/b/isbn/9780316769174-M.jpg",
                    _ => book.ImageUrl
                };
            }

            await _context.SaveChangesAsync();
            
            // Clear cache for all books
            var cacheKeys = books.Select(b => $"book:{b.Id}").ToList();
            cacheKeys.Add("books:page-1:size-10:category-all:search-all");
            
            foreach (var key in cacheKeys)
            {
                await _cacheService.RemoveAsync(key);
            }
            
            return true;
        }

        private static BookDto MapToDto(Book book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Price = book.Price,
                Description = book.Description,
                Category = book.Category,
                StockQuantity = book.StockQuantity,
                ImageUrl = book.ImageUrl
            };
        }
    }
}
