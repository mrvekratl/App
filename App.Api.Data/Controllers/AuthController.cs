using App.Api.Data.Models.ViewModels;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace App.Api.Data.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly DataRepository<UserEntity> _userRepo;

        public AuthController(DataRepository<UserEntity> userRepo)
        {
            _userRepo = userRepo;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepo.GetAll()
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == loginModel.Email && u.Password == loginModel.Password);

            if (user == null)
                return Unauthorized(new { Message = "Kullanıcı adı veya şifre hatalı." });

            if (!user.Enabled)
                return Unauthorized(new { Message = "Hesabınız aktif değil. Lütfen yöneticinizle iletişime geçin." });

            // Kullanıcı bilgilerini ve rollerini Claim olarak alıyoruz.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            // JWT token oluşturuyoruz
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSecretKey"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "yourIssuer",
                audience: "yourAudience",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserViewModel newUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new UserEntity
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                Password = newUser.Password,
                RoleId = 3, // Varsayılan olarak "Customer" rolü atanır.
                Enabled = true
            };

            await _userRepo.AddAsync(user);

            return Ok(new { Message = "Kayıt işlemi başarılı. Giriş yapabilirsiniz." });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("YourCookieName");  // JWT token'ı içeren cookie'yi siliyoruz.
            return Ok(new { Message = "Başarıyla çıkış yaptınız." });
        }

    }

}

