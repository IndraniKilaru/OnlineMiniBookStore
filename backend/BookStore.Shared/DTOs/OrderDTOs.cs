using BookStore.Shared.Models;

namespace BookStore.Shared.DTOs
{    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
    
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public BookDto? Book { get; set; }
    }
    
    public class CreateOrderDto
    {
        public List<OrderItemRequestDto> Items { get; set; } = new();
    }
    
    public class OrderItemRequestDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }
}
