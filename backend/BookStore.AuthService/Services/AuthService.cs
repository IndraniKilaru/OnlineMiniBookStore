using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BookStore.AuthService.Data;
using BookStore.Shared.Models;
using BookStore.Shared.Services;
using BookStore.Shared.Events;

namespace BookStore.AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public AuthService(AuthDbContext context, IConfiguration configuration, IMessageBus messageBus)
        {
            _context = context;
            _configuration = configuration;
            _messageBus = messageBus;
            _jwtSecret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
            _jwtIssuer = _configuration["Jwt:Issuer"] ?? "BookStore";
            _jwtAudience = _configuration["Jwt:Audience"] ?? "BookStore";
        }

        public async Task<(bool Success, string Token, string RefreshToken, User? User)> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return (false, string.Empty, string.Empty, null);
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            return (true, token, refreshToken, user);
        }        public async Task<(bool Success, string Message, User? User)> RegisterAsync(string firstName, string lastName, string email, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                return (false, "Email already exists", null);
            }

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Customer",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Publish user registered event
            var userRegisteredEvent = new UserRegisteredEvent
            {
                UserId = user.Id.ToString(),
                Username = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                RegisteredAt = user.CreatedAt
            };

            await _messageBus.PublishAsync("bookstore.users", "user.registered", userRegisteredEvent);

            return (true, "User registered successfully", user);
        }        public async Task<(bool Success, string Message, User? User)> RegisterAdminAsync(string firstName, string lastName, string email, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                return (false, "Email already exists", null);
            }

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Publish user registered event
            var userRegisteredEvent = new UserRegisteredEvent
            {
                UserId = user.Id.ToString(),
                Username = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                RegisteredAt = user.CreatedAt
            };

            await _messageBus.PublishAsync("bookstore.users", "user.registered", userRegisteredEvent);

            return (true, "Admin user registered successfully", user);
        }        public (bool Success, string Token, string RefreshToken) RefreshToken(string refreshToken)
        {
            // In a real implementation, you would validate the refresh token against a database
            // For simplicity, we'll just generate new tokens
            return (true, string.Empty, GenerateRefreshToken());
        }        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret);
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
