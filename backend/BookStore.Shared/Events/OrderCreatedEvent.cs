using System;

namespace BookStore.Shared.Events
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public List<OrderItemEvent> Items { get; set; } = new();
    }

    public class OrderItemEvent
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
