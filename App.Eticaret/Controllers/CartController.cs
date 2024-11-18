using App.Data.Entities;
using App.Data.Infrastructure;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "buyer, seller")]
    public class CartController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("/add-to-cart/{productId:int}")]
        public async Task<IActionResult> AddProduct([FromRoute] int productId)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            // Ürün var mı kontrolü
            var client = _httpClientFactory.CreateClient("ProductService");
            var productExists = await client.GetFromJsonAsync<bool>($"/api/products/exists/{productId}");

            if (!productExists)
            {
                return NotFound("Product does not exist.");
            }

            // Cart API'ye ürün ekleme
            var addCartItemResponse = await client.PostAsJsonAsync("/api/cart-items/add", new
            {
                UserId = userId,
                ProductId = productId,
                Quantity = 1
            });

            if (!addCartItemResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)addCartItemResponse.StatusCode, await addCartItemResponse.Content.ReadAsStringAsync());
            }

            return RedirectToAction(nameof(Edit));
        }

        [HttpGet("/cart")]
        public async Task<IActionResult> Edit()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            // Sepet içeriği API'den alınıyor
            var client = _httpClientFactory.CreateClient("CartService");
            var cartItemsResponse = await client.GetFromJsonAsync<List<CartItemViewModel>>($"/api/cart-items/{userId}");

            return View(cartItemsResponse);
        }

        [HttpGet("/cart/{cartItemId:int}/remove")]
        public async Task<IActionResult> Remove([FromRoute] int cartItemId)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            // Cart API'ye ürün silme isteği
            var client = _httpClientFactory.CreateClient("CartService");
            var removeResponse = await client.DeleteAsync($"/api/cart-items/{cartItemId}");

            if (!removeResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)removeResponse.StatusCode, await removeResponse.Content.ReadAsStringAsync());
            }

            return RedirectToAction(nameof(Edit));
        }

        [HttpPost("/cart/update")]
        public async Task<IActionResult> UpdateCart(int cartItemId, byte quantity)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            // Cart API'ye güncelleme isteği
            var client = _httpClientFactory.CreateClient("CartService");
            var response = await client.PutAsJsonAsync("/api/cart-items/update", new { CartItemId = cartItemId, Quantity = quantity });

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return RedirectToAction(nameof(Edit));
        }

        [HttpGet("/checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            // API'ye istek gönderiyoruz
            var client = _httpClientFactory.CreateClient("CartService");
            var cartItemsResponse = await client.GetFromJsonAsync<List<CartItemViewModel>>($"/api/cart-items/{userId}");

            if (cartItemsResponse == null || !cartItemsResponse.Any())
            {
                return NotFound("Your cart is empty.");
            }

            return View(cartItemsResponse);
        }
    }
}