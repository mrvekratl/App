using App.Data;
using App.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using App.Data.Infrastructure;
using App.Api.Data.Models.ViewModels;

namespace App.Data.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductApiController : ControllerBase
    {
        private readonly DataRepository<ProductEntity> _productRepo;
        private readonly DataRepository<ProductImageEntity> _productImageRepo;
        private readonly DataRepository<ProductCommentEntity> _productCommentRepo;

        public ProductApiController(
            DataRepository<ProductEntity> productRepo,
            DataRepository<ProductImageEntity> productImageRepo,
            DataRepository<ProductCommentEntity> productCommentRepo)
        {
            _productRepo = productRepo;
            _productImageRepo = productImageRepo;
            _productCommentRepo = productCommentRepo;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productRepo.GetAll().ToListAsync();
            return Ok(products);
        }

        // GET: api/product/{id}
        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> CreateProduct([FromBody] SaveProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newProduct = new ProductEntity
            {
                SellerId = model.SellerId,
                CategoryId = model.CategoryId,
                DiscountId = model.DiscountId,
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                StockAmount = model.StockAmount
            };

            var product = await _productRepo.AddAsync(newProduct);
            return CreatedAtAction(nameof(GetProductById), new { productId = product.Id }, product);
        }

        // PUT: api/product/{id}
        [HttpPut("{productId:int}")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] SaveProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.StockAmount = model.StockAmount;
            product.CategoryId = model.CategoryId;
            product.DiscountId = model.DiscountId;

            await _productRepo.UpdateAsync(product);
            return Ok(product);
        }

        // DELETE: api/product/{id}
        [HttpDelete("{productId:int}")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepo.DeleteAsync(productId);
            return NoContent();
        }

        // POST: api/product/{productId}/images
        [HttpPost("{productId:int}/images")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> UploadProductImage(int productId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { file.FileName });
        }

        // POST: api/product/{productId}/comment
        [HttpPost("{productId:int}/comment")]
        [Authorize(Roles = "buyer, seller")]
        public async Task<IActionResult> AddProductComment(int productId, [FromBody] SaveProductCommentViewModel commentModel)
        {
            var userId = User.Identity.Name; // Kullanıcı ID'si

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound("Ürün bulunamadı.");
            }

            var comment = new ProductCommentEntity
            {
                ProductId = productId,
                Text = commentModel.Text,
                StarCount = commentModel.StarCount
            };

            await _productCommentRepo.AddAsync(comment);

            return Ok(new { message = "Yorum başarıyla eklendi." });
        }
    }
}

