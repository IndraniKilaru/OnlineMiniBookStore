using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookStore.BooksService.Services;
using BookStore.Shared.DTOs;
using BookStore.Shared.Common;

namespace BookStore.BooksService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;

        public BooksController(IBooksService booksService)
        {
            _booksService = booksService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<BookDto>>>> GetBooks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? category = null,
            [FromQuery] string? searchTerm = null)
        {
            var result = await _booksService.GetBooksAsync(page, pageSize, category, searchTerm);
            return Ok(ApiResponse<PagedResult<BookDto>>.SuccessResponse(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<BookDto>>> GetBook(int id)
        {
            var book = await _booksService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound(ApiResponse<BookDto>.ErrorResponse("Book not found"));
            }

            return Ok(ApiResponse<BookDto>.SuccessResponse(book));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<BookDto>>> CreateBook(CreateBookDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<BookDto>.ErrorResponse("Invalid input", errors));
            }

            var book = await _booksService.CreateBookAsync(bookDto);
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, 
                ApiResponse<BookDto>.SuccessResponse(book, "Book created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<BookDto>>> UpdateBook(int id, UpdateBookDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<BookDto>.ErrorResponse("Invalid input", errors));
            }

            var book = await _booksService.UpdateBookAsync(id, bookDto);
            if (book == null)
            {
                return NotFound(ApiResponse<BookDto>.ErrorResponse("Book not found"));
            }

            return Ok(ApiResponse<BookDto>.SuccessResponse(book, "Book updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteBook(int id)
        {
            var success = await _booksService.DeleteBookAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Book not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Book deleted successfully"));
        }        [HttpGet("categories")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetCategories()
        {
            var categories = await _booksService.GetCategoriesAsync();
            return Ok(ApiResponse<List<string>>.SuccessResponse(categories));
        }

        [HttpPost("update-image-urls")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateImageUrls()
        {
            var success = await _booksService.UpdateImageUrlsAsync();
            if (success)
            {
                return Ok(ApiResponse<string>.SuccessResponse("Updated", "Image URLs updated successfully"));
            }
            return BadRequest(ApiResponse<string>.ErrorResponse("Failed to update image URLs"));
        }
    }
}
