using System.ComponentModel.DataAnnotations;

namespace BookStore.Shared.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
    
    public class CreateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string ISBN { get; set; } = string.Empty;
        
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        public string Category { get; set; } = string.Empty;
        
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
    }
    
    public class UpdateBookDto
    {
        [StringLength(200)]
        public string? Title { get; set; }
        
        [StringLength(100)]
        public string? Author { get; set; }
        
        [StringLength(20)]
        public string? ISBN { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public string? Category { get; set; }
        
        [Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }
        
        public string? ImageUrl { get; set; }
    }
}
