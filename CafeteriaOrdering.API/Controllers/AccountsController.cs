using CafeteriaOrdering.API.DTO;
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
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDTO)
        {
            try
            {
                // Kiểm tra trong DB của bạn
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);

                if (user == null || user.Password != loginDTO.Password)
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
                    new Claim("Role", user.Role.ToString()),
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
                var response = new LoginResponseDTO
                {
                    Token = token,
                    Role = user.Role,
                    AccountId = user.UserId.ToString()
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Kiểm tra trùng email
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == registerDTO.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { Message = "Email đã được sử dụng. Vui lòng chọn email khác." });
                }

                var newUser = new User
                {
                    FullName = registerDTO.FullName,
                    Email = registerDTO.Email,
                    Password = registerDTO.Password,
                    Phone = registerDTO.Phone,
                    Role = registerDTO.Role,
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
