using App.Admin.Models.ViewModels;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Admin.Controllers
{
    [Route("/categories")]
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;
        public CategoryController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var categories = await _httpClient.GetFromJsonAsync<List<CategoryListViewModel>>("api/categories");
            return View(categories);
        }

        [Route("create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SaveCategoryViewModel newCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return View(newCategoryModel);
            }

            var response = await _httpClient.PostAsJsonAsync("api/categories", newCategoryModel);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Kategori başarıyla oluşturuldu.";
                ModelState.Clear();
                return View();
            }

            ViewBag.ErrorMessage = "Bir hata oluştu. Lütfen tekrar deneyin.";
            return View(newCategoryModel);
        }

        [Route("{categoryId:int}/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int categoryId)
        {
            var category = await _httpClient.GetFromJsonAsync<SaveCategoryViewModel>($"api/categories/{categoryId}");
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [Route("{categoryId:int}/edit")]
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int categoryId, [FromForm] SaveCategoryViewModel editCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editCategoryModel);
            }

            var response = await _httpClient.PutAsJsonAsync($"api/categories/{categoryId}", editCategoryModel);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Kategori başarıyla güncellendi.";
                ModelState.Clear();
                return View(editCategoryModel);
            }

            ViewBag.ErrorMessage = "Bir hata oluştu. Lütfen tekrar deneyin.";
            return View(editCategoryModel);
        }

        [Route("{categoryId:int}/delete")]
        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int categoryId)
        {
            var response = await _httpClient.DeleteAsync($"api/categories/{categoryId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(List));
            }

            ViewBag.ErrorMessage = "Silme işlemi başarısız oldu.";
            return RedirectToAction(nameof(List));
        }
    }
}