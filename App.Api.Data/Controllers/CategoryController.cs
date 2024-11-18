using App.Api.Data.Models;
using App.Api.Data.Models.ViewModels;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class CategoryController : ControllerBase
    {
        private readonly DataRepository<CategoryEntity> _categoryRepo;

        public CategoryController(DataRepository<CategoryEntity> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryRepo.GetAll()
                .Select(c => new CategoryListViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.Color,
                    IconCssClass = c.IconCssClass
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/categories/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryListViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color,
                IconCssClass = category.IconCssClass
            };

            return Ok(viewModel);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] SaveCategoryViewModel newCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryEntity = new CategoryEntity
            {
                Name = newCategoryModel.Name,
                Color = newCategoryModel.Color,
                IconCssClass = newCategoryModel.IconCssClass
            };

            await _categoryRepo.AddAsync(categoryEntity);

            return CreatedAtAction(nameof(GetCategoryById), new { id = categoryEntity.Id }, categoryEntity);
        }

        // PUT: api/categories/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] SaveCategoryViewModel editCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = editCategoryModel.Name;
            category.Color = editCategoryModel.Color;
            category.IconCssClass = editCategoryModel.IconCssClass;

            await _categoryRepo.UpdateAsync(category);

            return NoContent();
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _categoryRepo.DeleteAsync(id);

            return NoContent();
        }
    }
}

