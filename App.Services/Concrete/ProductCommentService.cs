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
    public class ProductCommentService : IProductCommentService
    {
        private readonly HttpClient _client;

        public ProductCommentService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("EcommerceApi");
        }

        public async Task<Result<List<ProductCommentDto>>> GetCommentsByProductIdAsync(int productId)
        {
            var response = await _client.GetAsync($"api/products/{productId}/comments");

            if (!response.IsSuccessStatusCode)
            {
                return Result<List<ProductCommentDto>>.Fail("Unable to fetch product comments.");
            }

            var comments = await response.Content.ReadFromJsonAsync<List<ProductCommentDto>>();
            return Result<List<ProductCommentDto>>.Success(comments);
        }

        public async Task<Result<ProductCommentDto>> CreateCommentAsync(ProductCommentDto commentDto)
        {
            var response = await _client.PostAsJsonAsync("api/productcomments", commentDto);

            if (!response.IsSuccessStatusCode)
            {
                return Result<ProductCommentDto>.Fail("Unable to create comment.");
            }

            var createdComment = await response.Content.ReadFromJsonAsync<ProductCommentDto>();
            return Result<ProductCommentDto>.Success(createdComment);
        }

        public async Task<Result<ProductCommentDto>> UpdateCommentAsync(int commentId, ProductCommentDto commentDto)
        {
            var response = await _client.PutAsJsonAsync($"api/productcomments/{commentId}", commentDto);

            if (!response.IsSuccessStatusCode)
            {
                return Result<ProductCommentDto>.Fail("Unable to update comment.");
            }

            var updatedComment = await response.Content.ReadFromJsonAsync<ProductCommentDto>();
            return Result<ProductCommentDto>.Success(updatedComment);
        }

        public async Task<Result> DeleteCommentAsync(int commentId)
        {
            var response = await _client.DeleteAsync($"api/productcomments/{commentId}");

            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail("Unable to delete comment.");
            }

            return Result.Success();
        }
    }

}
