using Microsoft.AspNetCore.Mvc;
using BookApi.Models;
using Microsoft.AspNetCore.Identity;

namespace BookApi.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;


        public AuthService(UserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

       

        public async Task<string?> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userService.GetUserByEmailAsync(loginRequest.Email);
            if (user == null)
            return null;

            // Verify the password
            var hasher = new PasswordHasher<User>();
           var result = hasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);
            if (result == PasswordVerificationResult.Success)
            {
            var token = _jwtService.GenerateToken(user.Id, user.Email);
            return token;
            }
            return null; // Invalid password
            
        }

        public async Task<bool> RegisterAsync(RegisterRequest registerRequest)
        {
            var existingUser = await _userService.GetUserByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                return false; // User already exists
            }

            var hasher = new PasswordHasher<User>();

            var newUser = new User
            {
                Email = registerRequest.Email,
                Username = registerRequest.Username
            };

            newUser.PasswordHash = hasher.HashPassword(newUser, registerRequest.Password);

            Console.WriteLine("Saving user to DB...");
            await _userService.CreateUserAsync(newUser);
            Console.WriteLine("User saved!");
            return true;
        }
    }
}