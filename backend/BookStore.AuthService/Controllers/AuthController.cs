using Microsoft.AspNetCore.Mvc;
using BookStore.AuthService.Services;
using BookStore.Shared.DTOs;
using BookStore.Shared.Common;

namespace BookStore.AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid input", errors));
            }

            var result = await _authService.LoginAsync(request.Email, request.Password);
            
            if (!result.Success)
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid credentials"));
            }

            var response = new AuthResponseDto
            {
                Token = result.Token,
                RefreshToken = result.RefreshToken,
                User = new UserDto
                {
                    Id = result.User!.Id,
                    FirstName = result.User.FirstName,
                    LastName = result.User.LastName,
                    Email = result.User.Email,
                    Role = result.User.Role
                }
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful"));
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid input", errors));
            }

            var result = await _authService.RegisterAsync(request.FirstName, request.LastName, request.Email, request.Password);
            
            if (!result.Success)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(result.Message));
            }

            var authResult = await _authService.LoginAsync(request.Email, request.Password);
            
            var response = new AuthResponseDto
            {
                Token = authResult.Token,
                RefreshToken = authResult.RefreshToken,
                User = new UserDto
                {
                    Id = result.User!.Id,
                    FirstName = result.User.FirstName,
                    LastName = result.User.LastName,
                    Email = result.User.Email,
                    Role = result.User.Role
                }
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Registration successful"));
        }

        [HttpPost("register-admin")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RegisterAdmin(RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid input", errors));
            }

            var result = await _authService.RegisterAdminAsync(request.FirstName, request.LastName, request.Email, request.Password);
            
            if (!result.Success)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(result.Message));
            }

            var authResult = await _authService.LoginAsync(request.Email, request.Password);
            
            var response = new AuthResponseDto
            {
                Token = authResult.Token,
                RefreshToken = authResult.RefreshToken,
                User = new UserDto
                {
                    Id = result.User!.Id,
                    FirstName = result.User.FirstName,
                    LastName = result.User.LastName,
                    Email = result.User.Email,
                    Role = result.User.Role
                }
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Admin registration successful"));
        }

        [HttpPost("refresh")]
        public ActionResult<ApiResponse<AuthResponseDto>> RefreshToken([FromBody] string refreshToken)
        {
            var result = _authService.RefreshToken(refreshToken);
            
            if (!result.Success)
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid refresh token"));
            }

            var response = new AuthResponseDto
            {
                Token = result.Token,
                RefreshToken = result.RefreshToken
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Token refreshed"));
        }

        [HttpPost("validate")]
        public ActionResult<ApiResponse<bool>> ValidateToken([FromBody] string token)
        {
            var isValid = _authService.ValidateToken(token);
            return Ok(ApiResponse<bool>.SuccessResponse(isValid));
        }
    }
}
