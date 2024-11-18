using App.Data.Entities;
using App.Data.Infrastructure;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace App.Eticaret.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("/register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("/register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterUserViewModel newUser)
        {
            if (!ModelState.IsValid)
            {
                return View(newUser);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync("api/auth/register", newUser);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Kayıt işlemi başarılı. Giriş yapabilirsiniz.";
                ModelState.Clear();
                return View();
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Kayıt işlemi başarısız: {error}");
            return View(newUser);
        }

        [Route("/login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync("api/auth/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserEntity>();
                await LogInAsync(user);
                return RedirectToAction("Index", "Home");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Giriş başarısız: {error}");
            return View(loginModel);
        }

        private async Task LogInAsync(UserEntity user)
        {
            if (user == null) return;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.Name),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            });
        }

        [Route("/forgot-password")]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("/forgot-password")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync("api/auth/forgot-password", model);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Şifre sıfırlama maili gönderildi. Lütfen e-posta adresinizi kontrol edin.";
                ModelState.Clear();
                return View();
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Hata: {error}");
            return View(model);
        }

        [Route("/renew-password/{verificationCode}")]
        [HttpGet]
        public async Task<IActionResult> RenewPassword([FromRoute] string verificationCode)
        {
            if (string.IsNullOrEmpty(verificationCode))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.GetAsync($"api/auth/renew-password/{verificationCode}");

            if (response.IsSuccessStatusCode)
            {
                return View();
            }

            return RedirectToAction(nameof(ForgotPassword));
        }

        [Route("/renew-password")]
        [HttpPost]
        public async Task<IActionResult> RenewPassword([FromForm] object model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");

            var response = await client.PostAsJsonAsync("api/auth/renew-password", model);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Şifreniz başarıyla sıfırlandı. Giriş yapabilirsiniz.";
                return RedirectToAction(nameof(Login));
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Hata: {error}");
            return View(model);
        }

        [Route("/logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
