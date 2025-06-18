using BookStore.Shared.Models;
using BookStore.Shared.DTOs;
using BookStore.Shared.Common;

namespace BookStore.BooksService.Services
{
    public interface IBooksService
    {        Task<PagedResult<BookDto>> GetBooksAsync(int page = 1, int pageSize = 10, string? category = null, string? searchTerm = null);
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto bookDto);
        Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto bookDto);
        Task<bool> DeleteBookAsync(int id);
        Task<List<string>> GetCategoriesAsync();
        Task<bool> UpdateImageUrlsAsync();
    }
}
