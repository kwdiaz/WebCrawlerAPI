using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebCrawlerAPI.Context;
using WebCrawlerAPI.Models;

namespace WebCrawlerAPI.Controllers
{
    [ApiController] 
    [Route("[controller]")] 
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration; // Configuration for JWT settings
        private readonly HackerNewsContext _context; // Database context

       
        public AuthController(IConfiguration configuration, HackerNewsContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // POST endpoint for user registration
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel user)
        {
            // Check if the username is already in use
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest("Username is already in use.");
            }

            // Hash the password
            var passwordHash = HashPassword(user.Password);

            // Create a new user model
            var newUser = new UserModel
            {
                Username = user.Username,
                PasswordHash = passwordHash, // Store the hashed password with the salt
                Email = user.Email
            };

            // Add the new user to the database
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return Ok("User registered successfully.");
        }

        // POST endpoint for user login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            // Find the existing user by username
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginModel.Username);
            // Check if user exists and verify the password
            if (existingUser == null || !VerifyPassword(loginModel.Password, existingUser.PasswordHash))
            {
                return Unauthorized("Invalid credentials.");
            }

            // Generate a JWT token for the user
            var token = GenerateJwtToken(existingUser.Username);
            return Ok(new { Token = token }); // Return the token
        }

        // Method to generate a JWT token
        private string GenerateJwtToken(string username)
        {
            var claims = new[] // Define claims for the token
            {
                new Claim(ClaimTypes.Name, username)
            };

            // Create a symmetric security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Signing credentials

            // Create a JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"])), // Set token expiration
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token); // Write and return the token
        }

        // Function to hash the password along with the salt
        private string HashPassword(string password)
        {
            using (var hmac = new HMACSHA512())
            {
                var salt = hmac.Key; // Generate a salt
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); // Hash the password with the salt
                return Convert.ToBase64String(hash) + "." + Convert.ToBase64String(salt); // Return the hash and salt
            }
        }

        // Function to verify the password
        private bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.'); // Split the stored hash to get hash and salt
            var hash = Convert.FromBase64String(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);

            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); // Compute the hash for the provided password
                return computedHash.SequenceEqual(hash); // Compare the computed hash with the stored hash
            }
        }

        // Model for user registration
        public class RegisterModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        // Model for user login
        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
