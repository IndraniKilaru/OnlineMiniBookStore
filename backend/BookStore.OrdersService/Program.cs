using Microsoft.EntityFrameworkCore;
using BookStore.OrdersService.Data;
using BookStore.OrdersService.Services;
using BookStore.Shared.DTOs;
using BookStore.Shared.Models;
using BookStore.Shared.Common;
using BookStore.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseInMemoryDatabase("OrdersDb"));

// Add RabbitMQ Message Bus
builder.Services.AddSingleton<IMessageBus, RabbitMQMessageBus>();

// Add Order Service
builder.Services.AddScoped<IOrderService, OrderServiceImpl>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowAll");

// Health check endpoint
app.MapGet("/health", () => "Orders Service is running")
.WithName("HealthCheck");

// Order Service endpoints
app.MapPost("/api/orders/{userId:int}", async (int userId, CreateOrderDto createOrderDto, IOrderService orderService) =>
{
    try
    {
        var order = await orderService.CreateOrderAsync(userId, createOrderDto);
        return Results.Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order created successfully"));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ApiResponse<OrderDto>.ErrorResponse($"Error creating order: {ex.Message}"));
    }
})
.WithName("CreateOrder");

app.MapGet("/api/orders/user/{userId:int}", async (int userId, IOrderService orderService) =>
{
    try
    {
        var orders = await orderService.GetUserOrdersAsync(userId);
        return Results.Ok(ApiResponse<List<OrderDto>>.SuccessResponse(orders));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ApiResponse<List<OrderDto>>.ErrorResponse($"Error getting user orders: {ex.Message}"));
    }
})
.WithName("GetUserOrders");

app.MapGet("/api/orders/{orderId:int}", async (int orderId, IOrderService orderService) =>
{
    try
    {
        var order = await orderService.GetOrderAsync(orderId);
        if (order == null)
            return Results.NotFound(ApiResponse<OrderDto>.ErrorResponse("Order not found"));
            
        return Results.Ok(ApiResponse<OrderDto>.SuccessResponse(order));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ApiResponse<OrderDto>.ErrorResponse($"Error getting order: {ex.Message}"));
    }
})
.WithName("GetOrder");

app.MapPut("/api/orders/{orderId:int}/status", async (int orderId, OrderStatus status, IOrderService orderService) =>
{
    try
    {
        var order = await orderService.UpdateOrderStatusAsync(orderId, status);
        if (order == null)
            return Results.NotFound(ApiResponse<OrderDto>.ErrorResponse("Order not found"));
            
        return Results.Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order status updated successfully"));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ApiResponse<OrderDto>.ErrorResponse($"Error updating order status: {ex.Message}"));
    }
})
.WithName("UpdateOrderStatus");

app.MapGet("/api/orders", async (IOrderService orderService) =>
{
    try
    {
        var orders = await orderService.GetAllOrdersAsync();
        return Results.Ok(ApiResponse<List<OrderDto>>.SuccessResponse(orders));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ApiResponse<List<OrderDto>>.ErrorResponse($"Error getting all orders: {ex.Message}"));
    }
})
.WithName("GetAllOrders");

app.Run();
