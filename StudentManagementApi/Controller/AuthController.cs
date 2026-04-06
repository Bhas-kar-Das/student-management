using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentManagementApi.Controller
{
    // Controller for authentication operations
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        // Configuration for JWT settings
        private readonly IConfiguration _configuration;

        // Logger instance
        private readonly ILogger<AuthController> _logger;

        // Constructor with dependency injection
        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // =====================================================
        // POST: api/auth/login
        // Login to get JWT token
        // =====================================================
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user: {Username}", request.Username);

            try
            {
                // Validate credentials - in production, use proper password hashing
                // For demo purposes, using hardcoded credentials
                if (!ValidateUser(request.Username, request.Password))
                {
                    _logger.LogWarning("Invalid login attempt for user: {Username}", request.Username);
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // Generate JWT token
                var token = GenerateJwtToken(request.Username);
                
                _logger.LogInformation("User {Username} logged in successfully", request.Username);
                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = request.Username,
                    ExpiresIn = 60 // Token expires in 60 minutes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", request.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Error during login", error = ex.Message });
            }
        }

        // =====================================================
        // Validate user credentials
        // In production, verify against database with hashed passwords
        // =====================================================
        private bool ValidateUser(string username, string password)
        {
            // Demo credentials - replace with database validation in production
            var validUsers = new Dictionary<string, string>
            {
                { "admin@example.com", "admin123" },
                { "user", "user123" }
            };

            return validUsers.TryGetValue(username.ToLower(), out var storedPassword) 
                   && storedPassword == password;
        }

        // =====================================================
        // Generate JWT token with claims
        // =====================================================
        private string GenerateJwtToken(string username)
        {
            // Get JWT settings from configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "YourSuperSecretKeyHere12345678901234567890"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create claims for the token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Create token descriptor
            var nowUtc = DateTime.UtcNow;
            var utcTime = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = utcTime,
                Expires = utcTime.AddMinutes(60),
                Issuer = _configuration["Jwt:Issuer"] ?? "StudentManagementApi",
                Audience = _configuration["Jwt:Audience"] ?? "StudentManagementApi",
                SigningCredentials = credentials
            };

            // Generate and return token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    // Request model for login
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // Response model for login
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }
}
