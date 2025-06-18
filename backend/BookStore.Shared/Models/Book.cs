using System.ComponentModel.DataAnnotations;

namespace BookStore.Shared.Models
{
    public class Book
    {
        public int Id { get; set; }
        
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
        
        public int StockQuantity { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
