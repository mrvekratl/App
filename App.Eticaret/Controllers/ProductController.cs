using App.Data.Entities;
using App.Data.Infrastructure;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Eticaret.Controllers
{
    [Route("/product")]
    public class ProductController: BaseController
    {
        private readonly HttpClient _httpClient;
        public ProductController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet("")]
        [Authorize(Roles = "seller")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Create([FromForm] SaveProductViewModel newProductModel)
        {
            if (!ModelState.IsValid)
            {
                return View(newProductModel);
            }

            var requestBody = new
            {
                newProductModel.Name,
                newProductModel.Price,
                newProductModel.Description,
                newProductModel.StockAmount,
                newProductModel.CategoryId,
                newProductModel.DiscountId
            };

            var response = await _httpClient.PostAsJsonAsync("/api/product", requestBody);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Ürün başarıyla eklendi.";
                ModelState.Clear();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Ürün eklerken bir hata oluştu.";
                return View(newProductModel);
            }
        }

        private async Task SaveProductImages(int productId, IList<IFormFile> images)
        {
            foreach (var image in images)
            {
                var formData = new MultipartFormDataContent();
                var imageContent = new StreamContent(image.OpenReadStream())
                {
                    Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg") }
                };
                formData.Add(imageContent, "file", image.FileName);

                var response = await _httpClient.PostAsync($"/api/product/{productId}/images", formData);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Resim yükleme sırasında bir hata oluştu.");
                }
            }
        }


        [HttpGet("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit([FromRoute] int productId)
        {
            var response = await _httpClient.GetAsync($"/api/product/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var productEntity = await response.Content.ReadAsAsync<ProductEntity>();

            if (productEntity.SellerId != GetUserId())
            {
                return Unauthorized();
            }

            var viewModel = new SaveProductViewModel
            {
                CategoryId = productEntity.CategoryId,
                DiscountId = productEntity.DiscountId,
                Name = productEntity.Name,
                Price = productEntity.Price,
                Description = productEntity.Description,
                StockAmount = productEntity.StockAmount
            };

            return View(viewModel);
        }

        [HttpPost("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit([FromRoute] int productId, [FromForm] SaveProductViewModel editProductModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editProductModel);
            }

            var requestBody = new
            {
                editProductModel.Name,
                editProductModel.Price,
                editProductModel.Description,
                editProductModel.StockAmount,
                editProductModel.CategoryId,
                editProductModel.DiscountId
            };

            var response = await _httpClient.PutAsJsonAsync($"/api/product/{productId}", requestBody);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Ürün başarıyla güncellendi.";
                return RedirectToAction("ProductDetail", "Home", new { productId });
            }
            else
            {
                ViewBag.ErrorMessage = "Ürün güncellenirken bir hata oluştu.";
                return View(editProductModel);
            }
        }

        [HttpGet("{productId:int}/delete")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Delete([FromRoute] int productId)
        {
            var response = await _httpClient.DeleteAsync($"/api/product/{productId}");

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Ürün başarıyla silindi.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Ürün silinirken bir hata oluştu.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("{productId:int}/comment")]
        [Authorize(Roles = "buyer, seller")]
        public async Task<IActionResult> Comment([FromRoute] int productId, [FromForm] SaveProductCommentViewModel newProductCommentModel)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var requestBody = new
            {
                newProductCommentModel.Text,
                newProductCommentModel.StarCount
            };

            var response = await _httpClient.PostAsJsonAsync($"/api/product/{productId}/comment", requestBody);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}