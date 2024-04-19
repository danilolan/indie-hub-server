using indie_hub_server.Dtos;
using indie_hub_server.Dtos.User;
using indie_hub_server.Models;
using indie_hub_server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace indie_hub_server.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto login)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == login.Email);

            if (user == null)
            {
                return BadRequest("User with this email not found"); ;
            }

            AuthService userService = new AuthService();
            if (!userService.VerifyPassword(user, login.Password, user.PasswordHash)) {
                return BadRequest("Wrong password");
            }

            var response = new AuthResponseDTO
            {
                Token = GenerateJwtToken(user),
            };

            return Ok(response);
        }

        [HttpPost("signup")]
        public async Task<ActionResult<UserResponseDTO>> SignUp([FromBody] SignUpDTO newUser)
        {
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == newUser.Email);
            if (emailExists)
            {
                return BadRequest("Email already in use.");
            }

            AuthService userService = new AuthService();
            var user = new User
            {
                Username = newUser.Username,
                Email = newUser.Email,
                PasswordHash = userService.HashPassword(null, newUser.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new AuthResponseDTO
            {
                Token = GenerateJwtToken(user),
            };

            return Ok(response);
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
