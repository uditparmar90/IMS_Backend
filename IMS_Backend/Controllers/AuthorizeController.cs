using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IMS_Backend.DBCommection;
using IMS_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace IMS_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthorizeController : ControllerBase
    {
        readonly IConfiguration _config;
        readonly MyApplicationDB _context;
        public AuthorizeController(IConfiguration config, MyApplicationDB context)
        {
            _config = config;
            _context = context;
        }
        private string GenerateJwtToken(ClsUsers users)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, users.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, users.Email),
                new Claim(ClaimTypes.Role, users.Role)
            };
            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT key is not configured.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, DateTime.UtcNow.AddMinutes(
              Convert.ToDouble(_config["Jwt:ValidDurationInMinutes"])), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            // 1. Fetch the actual user from the datab  ase
            // We use FirstOrDefault instead of Any() because we need the user object data (ID, Role) for the token.
            var dbUser = _context.ClsUsers
                .FirstOrDefault(u => u.Email == dto.Email && u.Password == dto.Password);

            // 2. Check if user exists
            if (dbUser == null)
            {
                return Unauthorized("Invalid credentials");
            }
            //HttpContext.Session.SetInt32("UserId", dbUser.ID);
            //HttpContext.Session.SetString("UserEmail", dbUser.Email);
            //HttpContext.Session.SetString("UserRole", dbUser.Role);

            // 3. Generate token using the Database User object (which has the ID and Role),
            // NOT the DTO (which only has Email and Password).
            var token = GenerateJwtToken(dbUser);

            return Ok(new { token });
        }
    }
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
