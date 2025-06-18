using Microsoft.EntityFrameworkCore;
using BookStore.Shared.Models;
using BookStore.Shared.DTOs;
using BookStore.OrdersService.Data;
using BookStore.Shared.Services;
using BookStore.Shared.Events;

namespace BookStore.OrdersService.Services
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createOrderDto);
        Task<List<OrderDto>> GetUserOrdersAsync(int userId);
        Task<OrderDto?> GetOrderAsync(int orderId);
        Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<List<OrderDto>> GetAllOrdersAsync();
    }

    public class OrderServiceImpl : IOrderService
    {
        private readonly OrdersDbContext _context;
        private readonly IMessageBus _messageBus;

        public OrderServiceImpl(OrdersDbContext context, IMessageBus messageBus)
        {
            _context = context;
            _messageBus = messageBus;
        }

        public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createOrderDto)
        {
            if (createOrderDto.Items == null || !createOrderDto.Items.Any())
                throw new ArgumentException("Order must contain at least one item");

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in createOrderDto.Items)
            {
                // In a real application, you would fetch book details from BooksService
                // For now, using mock prices
                var unitPrice = GetMockBookPrice(item.BookId);
                var totalPrice = unitPrice * item.Quantity;
                totalAmount += totalPrice;

                var orderItem = new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice
                };

                orderItems.Add(orderItem);
            }

            order.TotalAmount = totalAmount;
            order.OrderItems = orderItems;            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Publish order created event
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = userId.ToString(),
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                CustomerEmail = "customer@example.com", // This should come from user service
                Items = order.OrderItems.Select(oi => new OrderItemEvent
                {
                    BookId = oi.BookId,
                    BookTitle = $"Book {oi.BookId}", // This should come from book service
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };            await _messageBus.PublishAsync("bookstore.orders", "order.created", orderCreatedEvent);

            return MapToOrderDto(order);
        }        public async Task<List<OrderDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                orderDtos.Add(MapToOrderDto(order));
            }

            return orderDtos;
        }

        public async Task<OrderDto?> GetOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);            if (order == null)
                return null;

            return MapToOrderDto(order);
        }

        public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return null;

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return MapToOrderDto(order);
        }        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                orderDtos.Add(MapToOrderDto(order));
            }

            return orderDtos;
        }private OrderDto MapToOrderDto(Order order)
        {
            var orderDto = new OrderDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    BookId = oi.BookId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice,
                    Book = GetMockBookDetails(oi.BookId)
                }).ToList()
            };

            return orderDto;
        }private BookDto GetMockBookDetails(int bookId)
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

        private decimal GetMockBookPrice(int bookId)
        {
            // Mock pricing logic - in real app would call BooksService
            var prices = new Dictionary<int, decimal>
            {
                { 1, 12.99m },
                { 2, 14.99m },
                { 3, 13.99m },
                { 4, 11.99m },
                { 5, 13.50m }
            };

            return prices.ContainsKey(bookId) ? prices[bookId] : 15.99m;
        }
    }
}
