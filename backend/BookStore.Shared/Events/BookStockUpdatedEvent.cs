using System;

namespace BookStore.Shared.Events
{
    public class BookStockUpdatedEvent
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int QuantityChange { get; set; }
        public int NewStockLevel { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Reason { get; set; } = string.Empty; // "ORDER_PLACED", "STOCK_REPLENISHED", etc.
    }
}
