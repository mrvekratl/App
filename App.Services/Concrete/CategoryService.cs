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
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _client;

        public CategoryService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("EcommerceApi");
        }

        public async Task<Result<CategoryDto>> GetCategoryByIdAsync(int categoryId)
        {
            var response = await _client.GetAsync($"api/categories/{categoryId}");

            if (!response.IsSuccessStatusCode)
            {
                return Result<CategoryDto>.Fail("Unable to fetch category.");
            }

            var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
            return Result<CategoryDto>.Success(category);
        }

        public async Task<Result<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            var response = await _client.GetAsync("api/categories");

            if (!response.IsSuccessStatusCode)
            {
                return Result<List<CategoryDto>>.Fail("Unable to fetch categories.");
            }

            var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
            return Result<List<CategoryDto>>.Success(categories);
        }

        public async Task<Result<CategoryDto>> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var response = await _client.PostAsJsonAsync("api/categories", categoryDto);

            if (!response.IsSuccessStatusCode)
            {
                return Result<CategoryDto>.Fail("Unable to create category.");
            }

            var createdCategory = await response.Content.ReadFromJsonAsync<CategoryDto>();
            return Result<CategoryDto>.Success(createdCategory);
        }

        public async Task<Result<CategoryDto>> UpdateCategoryAsync(int categoryId, CategoryDto categoryDto)
        {
            var response = await _client.PutAsJsonAsync($"api/categories/{categoryId}", categoryDto);

            if (!response.IsSuccessStatusCode)
            {
                return Result<CategoryDto>.Fail("Unable to update category.");
            }

            var updatedCategory = await response.Content.ReadFromJsonAsync<CategoryDto>();
            return Result<CategoryDto>.Success(updatedCategory);
        }

        public async Task<Result> DeleteCategoryAsync(int categoryId)
        {
            var response = await _client.DeleteAsync($"api/categories/{categoryId}");

            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail("Unable to delete category.");
            }

            return Result.Success();
        }
    }

}
