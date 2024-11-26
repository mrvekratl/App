using App.Data.Entities;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    [Route("/product")]
    public class ProductController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
            var client = _httpClientFactory.CreateClient("Api.Data");
            var response = await client.PostAsJsonAsync("/product", newProductModel);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "An error occurred while creating the product. Please try again later.";
                return View(newProductModel);
            }

            SetSuccessMessage("Ürün başarıyla eklendi.");
            ModelState.Clear();

            return View();
        }

        [HttpGet("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit([FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var client = _httpClientFactory.CreateClient("Api.Data");
            var response = await client.GetAsync($"/product/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var productEntity = await response.Content.ReadFromJsonAsync<ProductEntity>();

            if (productEntity is null)
            {
                return NotFound();
            }

            if (productEntity.SellerId != GetUserId())
            {
                return Forbid();
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
            var client = _httpClientFactory.CreateClient("Api.Data");
            var response = await client.GetAsync($"/product/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var productEntity = await response.Content.ReadFromJsonAsync<ProductEntity>();

            if (productEntity is null)
            {
                return NotFound();
            }

            if (productEntity.SellerId != GetUserId())
            {
                return Forbid();
            }

            productEntity.CategoryId = editProductModel.CategoryId;
            productEntity.DiscountId = editProductModel.DiscountId;
            productEntity.Name = editProductModel.Name;
            productEntity.Price = editProductModel.Price;
            productEntity.Description = editProductModel.Description;
            productEntity.StockAmount = editProductModel.StockAmount;

            response = await client.PutAsJsonAsync($"/product/{productId}", productEntity);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "An error occurred while updating the product. Please try again later.";
                return View(editProductModel);
            }

            ViewBag.SuccessMessage = "Ürün başarıyla güncellendi.";
            return View(editProductModel);
        }

        [HttpGet("{productId:int}/delete")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Delete([FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }
            var client = _httpClientFactory.CreateClient("Api.Data");
            var response = await client.GetAsync($"/product/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var productEntity = await response.Content.ReadFromJsonAsync<ProductEntity>();

            if (productEntity is null)
            {
                return NotFound();
            }

            if (productEntity.SellerId != userId)
            {
                return Forbid();
            }

            response = await client.DeleteAsync($"/product/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "An error occurred while deleting the product. Please try again later.";
                return View();
            }

            SetSuccessMessage("Ürün başarıyla silindi.");
            return View();
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
            var client = _httpClientFactory.CreateClient("Api.Data");
            var response = await client.PostAsJsonAsync($"/products/{productId}/comment", newProductCommentModel);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}