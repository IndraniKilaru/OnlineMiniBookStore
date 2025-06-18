using BookStore.Shared.Models;

namespace BookStore.AuthService.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Token, string RefreshToken, User? User)> LoginAsync(string email, string password);
        Task<(bool Success, string Message, User? User)> RegisterAsync(string firstName, string lastName, string email, string password);
        Task<(bool Success, string Message, User? User)> RegisterAdminAsync(string firstName, string lastName, string email, string password);
        (bool Success, string Token, string RefreshToken) RefreshToken(string refreshToken);
        bool ValidateToken(string token);
        Task<User?> GetUserByIdAsync(int userId);
    }
}
