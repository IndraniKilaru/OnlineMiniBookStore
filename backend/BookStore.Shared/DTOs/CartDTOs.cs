using System.ComponentModel.DataAnnotations;

namespace BookStore.Shared.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public BookDto? Book { get; set; }
    }
    
    public class AddToCartDto
    {
        [Required]
        public int BookId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }
    
    public class UpdateCartItemDto
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
    
    public class CartSummaryDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }
}
