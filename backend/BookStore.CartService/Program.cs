using Microsoft.EntityFrameworkCore;
using BookStore.CartService.Data;
using BookStore.CartService.Services;
using BookStore.Shared.DTOs;
using BookStore.Shared.Common;
using BookStore.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseInMemoryDatabase("CartDb"));

// Add Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
});

// Register cache service
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Add Cart Service
builder.Services.AddScoped<ICartService, CartServiceImpl>();

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
app.MapGet("/health", () => "Cart Service is running")
.WithName("HealthCheck");

// Cart Service endpoints
app.MapGet("/api/cart/{userId:int}", async (int userId, ICartService cartService) =>
{
    try
    {
        var cart = await cartService.GetCartAsync(userId);        return Results.Ok(ApiResponse<CartSummaryDto>.SuccessResponse(cart));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ApiResponse<CartSummaryDto>.ErrorResponse($"Error getting cart: {ex.Message}"));
    }
})
.WithName("GetCart");

app.MapPost("/api/cart/{userId:int}/items", async (int userId, AddToCartDto addToCartDto, ICartService cartService) =>
{
    try
    {
        var cartItem = await cartService.AddToCartAsync(userId, addToCartDto);
        return Results.Ok(ApiResponse<CartItemDto>.SuccessResponse(cartItem));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ApiResponse<CartItemDto>.ErrorResponse($"Error adding to cart: {ex.Message}"));
    }
})
.WithName("AddToCart");

app.MapPut("/api/cart/{userId:int}/items/{itemId:int}", async (int userId, int itemId, UpdateCartItemDto updateDto, ICartService cartService) =>
{
    try
    {
        var cartItem = await cartService.UpdateCartItemAsync(userId, itemId, updateDto);
        return Results.Ok(ApiResponse<CartItemDto>.SuccessResponse(cartItem));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ApiResponse<CartItemDto>.ErrorResponse($"Error updating cart item: {ex.Message}"));
    }
})
.WithName("UpdateCartItem");

app.MapDelete("/api/cart/{userId:int}/items/{itemId:int}", async (int userId, int itemId, ICartService cartService) =>
{
    try
    {
        var success = await cartService.RemoveFromCartAsync(userId, itemId);
        if (success)
            return Results.Ok(ApiResponse<bool>.SuccessResponse(true, "Item removed from cart"));
        else
            return Results.NotFound(ApiResponse<bool>.ErrorResponse("Cart item not found"));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ApiResponse<bool>.ErrorResponse($"Error removing cart item: {ex.Message}"));
    }
})
.WithName("RemoveFromCart");

app.MapDelete("/api/cart/{userId:int}", async (int userId, ICartService cartService) =>
{
    try
    {
        await cartService.ClearCartAsync(userId);
        return Results.Ok(ApiResponse<bool>.SuccessResponse(true, "Cart cleared"));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ApiResponse<bool>.ErrorResponse($"Error clearing cart: {ex.Message}"));
    }
})
.WithName("ClearCart");

app.Run();
