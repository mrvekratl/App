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
    public class ProductService : IProductService
    {
        private readonly HttpClient _client;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("EcommerceApi");
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(int productId)
        {
            var response = await _client.GetAsync($"api/products/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return Result<ProductDto>.Fail("Unable to fetch product.");
            }

            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            return Result<ProductDto>.Success(product);
        }

        public async Task<Result<List<ProductDto>>> GetAllProductsAsync()
        {
            var response = await _client.GetAsync("api/products");

            if (!response.IsSuccessStatusCode)
            {
                return Result<List<ProductDto>>.Fail("Unable to fetch products.");
            }

            var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
            return Result<List<ProductDto>>.Success(products);
        }

        public async Task<Result<ProductDto>> CreateProductAsync(ProductDto productDto)
        {
            var response = await _client.PostAsJsonAsync("api/products", productDto);

            if (!response.IsSuccessStatusCode)
            {
                return Result<ProductDto>.Fail("Unable to create product.");
            }

            var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
            return Result<ProductDto>.Success(createdProduct);
        }

        public async Task<Result<ProductDto>> UpdateProductAsync(int productId, ProductDto productDto)
        {
            var response = await _client.PutAsJsonAsync($"api/products/{productId}", productDto);

            if (!response.IsSuccessStatusCode)
            {
                return Result<ProductDto>.Fail("Unable to update product.");
            }

            var updatedProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
            return Result<ProductDto>.Success(updatedProduct);
        }

        public async Task<Result> DeleteProductAsync(int productId)
        {
            var response = await _client.DeleteAsync($"api/products/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail("Unable to delete product.");
            }

            return Result.Success();
        }
    }
}

