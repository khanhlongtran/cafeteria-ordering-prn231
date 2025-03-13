using CafeteriaOrdering.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CafeteriaOrdering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly CafeteriaOrderingDBContext _dbContext;
        public AccountsController(CafeteriaOrderingDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] IDictionary<string, object> request)
        {
            try
            {
                string email = request["email"]?.ToString() ?? throw new ArgumentException("Email is required");
                string password = request["password"]?.ToString() ?? throw new ArgumentException("Password is required");

                // Kiểm tra trong DB của bạn
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null || user.Password != password)
                {
                    return Unauthorized(new { Message = "Email hoặc mật khẩu không đúng" });
                }

                //Generate JWT Token
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Role", user.Role),
                    new Claim("AccountId", user.UserId.ToString())
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var preparedToken = new JwtSecurityToken(
                    issuer: configuration["JwtConfig:Issuer"],
                    audience: configuration["JwtConfig:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

                var token = new JwtSecurityTokenHandler().WriteToken(preparedToken);
                var role = user.Role;
                var userId = user.UserId.ToString();
                return Ok(new
                {
                    Message = "Đăng nhập thành công",
                    Token = token,
                    Role = role,
                    UserId = userId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] IDictionary<string, object> request)
        {
            try
            {
                string fullName = request["fullName"]?.ToString() ?? throw new ArgumentException("FullName is required");
                string email = request["email"]?.ToString() ?? throw new ArgumentException("Email is required");
                string password = request["password"]?.ToString() ?? throw new ArgumentException("Password is required");
                string? phone = request.ContainsKey("phone") ? request["phone"]?.ToString() : null;
                string role = request["role"]?.ToString() ?? throw new ArgumentException("Role is required");
                //check dup account
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    return BadRequest(new { Message = "Email đã được sử dụng. Vui lòng chọn email khác." });
                }
                var newUser = new User
                {
                    FullName = fullName,
                    Email = email,
                    Password = password,
                    Phone = phone,
                    Role = role,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    DefaultCuisine = ""
                };

                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Message = "Đăng ký thành công", UserId = newUser.UserId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message, InnerException = ex.InnerException?.Message });
            }
        }
    }
}
