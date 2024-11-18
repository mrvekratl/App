using App.Data.Entities;
using App.Data.Infrastructure;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "seller, buyer")]
    public class ProfileController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet("/profile")]
        public async Task<IActionResult> Details()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/profile/details?userId={userId.Value}");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userViewModel = await response.Content.ReadFromJsonAsync<ProfileDetailsViewModel>();

            if (userViewModel == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            string? previousSuccessMessage = TempData["SuccessMessage"]?.ToString();

            if (previousSuccessMessage is not null)
            {
                SetSuccessMessage(previousSuccessMessage);
            }

            return View(userViewModel);
        }

        [HttpPost("/profile")]
        public async Task<IActionResult> Edit([FromForm] ProfileDetailsViewModel editMyProfileModel)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await GetCurrentUserAsync();

            if (user is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(editMyProfileModel);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("/api/profile/edit", editMyProfileModel);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Failed to update profile.");
            }

            TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
            return RedirectToAction(nameof(Details));
        }

        [HttpGet("/my-orders")]
        public async Task<IActionResult> MyOrders()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/profile/my-orders?userId={userId.Value}");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Auth");
            }

            var orders = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>();

            return View(orders);
        }

        [HttpGet("/my-products")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> MyProducts()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/profile/my-products?userId={userId.Value}");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Auth");
            }

            var products = await response.Content.ReadFromJsonAsync<List<MyProductsViewModel>>();

            return View(products);
        }
    }
}