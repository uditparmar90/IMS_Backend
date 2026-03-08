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
    public class AuthorizeController(IConfiguration config, MyApplicationDB context) : ControllerBase
    {

        readonly IConfiguration _config=config;
        readonly MyApplicationDB _context= context;
        private string GenerateJwtToken(ClsUsers users)
        {
            var jwtKey = _config["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ValidDurationInMinutes"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                
                    new Claim(JwtRegisteredClaimNames.Sub, users.ID.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, users.Email),
                    new Claim(ClaimTypes.Role, users.Role),
                    new Claim(ClaimTypes.NameIdentifier, users.ID.ToString()),
                    // Manually adding Jti (unique ID) can help with validation
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ]),
                Expires = expiration,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            // 1. Fetch the actual user from the datab  ase
            // We use FirstOrDefault instead of Any() because we need the user object data (ID, Role) for the token.
            var dbUser = _context.ClsUsers.FirstOrDefault(u => u.Email == dto.Email && u.Password == dto.Password);

            // 2. Check if user exists
            if (dbUser == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // 3. Generate token using the Database User object (which has the ID and Role),
            // NOT the DTO (which only has Email and Password).
            var token = GenerateJwtToken(dbUser);
            var userData = new ClsUsers { ID = dbUser.ID, Name = dbUser.Name, Password = dbUser.Password, Email = dbUser.Email, Role = dbUser.Role };
            return Ok(new { token, userInfo = userData });
        }

        [AllowAnonymous]
        [HttpPost("AddNewUser")]
        public IActionResult AddUser([FromBody] AdduserDto user)
        {
            user.Email = user.Email.ToLower();
            var existingUser = _context.ClsUsers.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return Conflict("User with this email already exists.");
            }
            else
            {
                var newUser = new ClsUsers
                {
                    Email = user.Email,
                    Password = user.Password,
                    Name = user.Name,
                    Role = user.Role,
                    created = DateTime.Now
                };
                _context.ClsUsers.Add(newUser);
                _context.SaveChanges();
            }

            return Ok();
        }
        [HttpPost("EditUserProfile")]
        public IActionResult EditUserProfile([FromBody] ClsUsers user)
        {
            var existingUser = _context.ClsUsers.FirstOrDefault(u => u.ID == user.ID);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }
            else
            {
                existingUser.Name = user.Name;
                existingUser.Password = user.Password;
                //existingUser.Role = user.Role;
                //existingUser.Email = user.Email;
                _context.SaveChanges();

            }
            return Ok();
        }
    }

    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    public class AdduserDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
        public required string Role { get; set; }

    }
}
