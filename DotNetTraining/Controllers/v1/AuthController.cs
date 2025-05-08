using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using DotNetTraining.Domains.Dtos;
using DotNetTraining.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DotNetTraining.Domains.Entities;
using Microsoft.AspNetCore.Authorization;

namespace DotNetTraining.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(UserService userService, IConfiguration config, IMapper mapper)
        {
            _userService = userService;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByEmail(request.Email);
            if (user == null)
                return Unauthorized(new { message = "Email không tồn tại" });

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Sai mật khẩu" });

            // 🔐 Lấy cấu hình từ JwtTokenSetting
            var jwtSettings = _config.GetSection("JwtTokenSetting");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SymmetricSecurityKey"]!);
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role ?? "user")
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            var userDto = _mapper.Map<UserDto>(user);

            return Ok(new
            {
                token = jwt,
                user = userDto
            });
        }

    }
}

