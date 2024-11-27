using App.Models.DTO;
using App.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Concrete
{
    public class CartService : ICartService
    {
        private readonly HttpClient _client;

        public CartService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("CartApi");
        }

        public async Task<Result> AddToCartAsync(string jwt, CartItemDto cartItem)
        {
            var response = await _client.PostAsJsonAsync("api/cart", cartItem);
            return response.IsSuccessStatusCode ? Result.Success() : Result.Error("Failed to add item to cart");
        }

        public async Task<Result> RemoveFromCartAsync(string jwt, int productId)
        {
            var response = await _client.DeleteAsync($"api/cart/{productId}");
            return response.IsSuccessStatusCode ? Result.Success() : Result.Error("Failed to remove item from cart");
        }

        public async Task<Result<CartDetailsDto>> GetCartDetailsAsync(string jwt, Result result)
        {
            var response = await _client.GetAsync("api/cart");
            if (!response.IsSuccessStatusCode)
            {
                return (Result<CartDetailsDto>)result;
            }
            var cartDetails = await response.Content.ReadFromJsonAsync<CartDetailsDto>();
            return Result.Success(cartDetails);
        }

        public Task<Result<CartDetailsDto>> GetCartDetailsAsync(string jwt)
        {
            throw new NotImplementedException();
        }
    }
}
