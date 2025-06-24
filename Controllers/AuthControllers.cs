using Microsoft.AspNetCore.Mvc;
using BookApi.Services;
using BookApi.Models;

namespace BookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginRequest loginRequest)
        {
            var token = await _authService.LoginAsync(loginRequest);
            if (token == null)
            {
                return Unauthorized();
            }
            
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Ok(new { message = "Login successful" });
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            return Ok(new { message = "Logout successful" });
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            if (User.Identity?.IsAuthenticated == true)
                return Ok(new { authenticated = true });
            return Ok(new { authenticated = false });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequest registerRequest)
        {
            var result = await _authService.RegisterAsync(registerRequest);
            if (!result)
            {
                return BadRequest("Registration failed");
            }
            return Ok();
        }
    }
}