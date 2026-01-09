using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using saeom_backend.DTOs;
using saeom_backend.Helpers;
using saeom_backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace saeom_backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin()
        {
            if (await _context.TblUsers.AnyAsync())
                return BadRequest("Admin already exists.");

            PasswordHelper.CreatePasswordHash(
                "Admin@123",
                out string hash,
                out string salt
            );

            var admin = new TblUser
            {
                UserName = "admin",
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };

            _context.TblUsers.Add(admin);
            await _context.SaveChangesAsync();

            return Ok("Admin created");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _context.TblUsers
                .FirstOrDefaultAsync(u =>
                    u.UserName == dto.Username &&
                    u.IsActive);

            if (user == null)
            {
                return Ok(new ApiResponse<object>
                {
                    response_code = 0,
                    response_message = "Invalid credentials"
                });
            }

            var isValid = PasswordHelper.VerifyPassword(
                dto.Password,
                user.PasswordHash,
                user.PasswordSalt
            );

            if (!isValid)
            {
                return Ok(new ApiResponse<object>
                {
                    response_code = 0,
                    response_message = "Invalid credentials"
                });
            }

            var token = GenerateJwtToken(user);
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,              // false only for local HTTP
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(2)
            });
            return Ok(new ApiResponse<object>
            {
                response_code = 1,
                response_message = "success",
                data = new
                {
                    token,
                    userId = user.UserId,
                    username = user.UserName
                }
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // If you use refresh tokens or blacklist → handle here
            return Ok(new
            {
                response_code = 1,
                response_message = "Logged out successfully"
            });
        }

        private string GenerateJwtToken(TblUser user)
        {

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            //new Claim(ClaimTypes.Role, user.)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_config["Jwt:ExpireMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
