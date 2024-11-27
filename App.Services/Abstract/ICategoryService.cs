using App.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface ICategoryService
    {
        Task<Result<CategoryDto>> GetCategoryByIdAsync(int categoryId);
        Task<Result<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<Result<CategoryDto>> CreateCategoryAsync(CategoryDto categoryDto);
        Task<Result<CategoryDto>> UpdateCategoryAsync(int categoryId, CategoryDto categoryDto);
        Task<Result> DeleteCategoryAsync(int categoryId);
    }

}
