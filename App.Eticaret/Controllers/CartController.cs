using App.Data.Entities;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "buyer, seller")]
    public class CartController: BaseController
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
            var client = _httpClientFactory.CreateClient("Api.Data");

            var response = await client.GetAsync($"/products/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var product = await response.Content.ReadFromJsonAsync<ProductEntity>();

            if (product is null)
            {
                return NotFound();
            }
            
            response = await client.GetAsync($"/user/{userId}/cart/?productId={productId}");

            if (response.IsSuccessStatusCode)
            {
                var cartItem = await response.Content.ReadFromJsonAsync<CartItemEntity>();

                if (cartItem is not null)
                {
                    cartItem.Quantity++;
                }
                else
                {
                    cartItem = new CartItemEntity
                    {
                        UserId = userId.Value,
                        ProductId = productId,
                        Quantity = 1,
                        CreatedAt = DateTime.UtcNow
                    };

                    response = await client.PostAsJsonAsync("/user/cart", cartItem);

                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }
                }
            }
            else
            {
                return BadRequest();
            }

            var prevUrl = Request.Headers.Referer.FirstOrDefault();

            if (prevUrl is null)
            {
                return RedirectToAction(nameof(Edit));
            }

            return Redirect(prevUrl);
        }

        [HttpGet("/cart")]
        public async Task<IActionResult> Edit()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            List<CartItemViewModel> cartItem = await GetCartItemsAsync();

            return View(cartItem);
        }

        [HttpGet("/cart/{cartItemId:int}/remove")]
        public async Task<IActionResult> Remove([FromRoute] int cartItemId)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }
            var client = _httpClientFactory.CreateClient("Api.Data");

            var response = await client.GetAsync($"/user/{userId}/cart/{cartItemId}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var cartItem = await response.Content.ReadFromJsonAsync<CartItemEntity>();

            if (cartItem is null)
            {
                return NotFound();
            }

            response = await client.DeleteAsync($"/user/{userId}/cart/{cartItemId}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
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
            var client = _httpClientFactory.CreateClient("Api.Data");

            var response = await client.GetAsync($"/user/{userId}/cart/{cartItemId}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var cartItem = await response.Content.ReadFromJsonAsync<CartItemEntity>();

            if (cartItem is null)
            {
                return NotFound();
            }

            cartItem.Quantity = quantity;

            response = await client.PutAsJsonAsync($"/user/{userId}/cart/{cartItemId}", cartItem);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            var model = new CartItemViewModel
            {
                Id = cartItem.Id,
                ProductName = cartItem.Product.Name,
                ProductImage = cartItem.Product.Images.Count != 0 ? cartItem.Product.Images.First().Url : null,
                Quantity = cartItem.Quantity,
                Price = cartItem.Product.Price
            };

            return View(model);
        }

        [HttpGet("/checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            List<CartItemViewModel> cartItems = await GetCartItemsAsync();

            return View(cartItems);
        }

        private async Task<List<CartItemViewModel>> GetCartItemsAsync()
        {
            var userId = GetUserId() ?? -1;
            var client = _httpClientFactory.CreateClient("Api.Data");

            var response = await client.GetAsync($"/user/{userId}/cart");

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            var cartItems = await response.Content.ReadFromJsonAsync<List<CartItemViewModel>>();

            return cartItems ?? [];
        }
    }
}