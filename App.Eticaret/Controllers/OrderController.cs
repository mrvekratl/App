using App.Data.Entities;
using App.Data.Infrastructure;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "buyer, seller")]
    public class OrderController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpPost("/order")]
        public async Task<IActionResult> Create([FromForm] CheckoutViewModel model)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            if (!ModelState.IsValid)
            {
                var viewModel = await GetCartItemsAsync();
                return View(viewModel);
            }

            // API çağrısını yapıyoruz
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("/api/order", new { model.Address, UserId = userId.Value });

            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<OrderDetailsViewModel>();
                return RedirectToAction(nameof(Details), new { orderCode = order.OrderCode });
            }

            // API çağrısı başarısızsa hata mesajı göster
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);
            return View();
        }

        [HttpGet("/order/{orderCode}/details")]
        public async Task<IActionResult> Details([FromRoute] string orderCode)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/order/{orderCode}/details");

            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<OrderDetailsViewModel>();
                return View(order);
            }

            return NotFound();
        }

        private async Task<List<CartItemViewModel>> GetCartItemsAsync()
        {
            var userId = GetUserId() ?? -1;
            var apiUrl = $"https://your-api-url/api/orders/{userId}/details"; // API endpoint

            // HttpClient ile API'ye GET isteği gönderiyoruz.
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    // API'den hata alındığında ne yapılacağını belirleyebilirsiniz
                    throw new Exception("API request failed");
                }

                // API'den gelen veriyi CartItemViewModel listesine dönüştürmek
                var result = await response.Content.ReadAsAsync<OrderDetailsViewModel>();  // Burada gelen veriyi uygun modele deserialize ediyoruz.

                // Verileri dönüşümle, uygun formatta döndürüyoruz
                var cartItems = result.Items.Select(ci => new CartItemViewModel
                {
                    // CartItemViewModel özelliklerini OrderItemViewModel'den alıyoruz.
                    ProductName = ci.ProductName,
                    Quantity = (byte)ci.Quantity, // Miktar byte tipine dönüştürülüyor
                    Price = ci.UnitPrice,
                }).ToList();

                return cartItems;
            }
        }

    }
}
