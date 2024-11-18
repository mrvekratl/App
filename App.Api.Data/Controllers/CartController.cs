using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Data.Api.Controllers
{
    [ApiController]
    [Route("api/cart-items")]
    public class CartItemApiController : ControllerBase
    {
        private readonly DataRepository<CartItemEntity> _cartItemRepo;

        public CartItemApiController(DataRepository<CartItemEntity> cartItemRepo)
        {
            _cartItemRepo = cartItemRepo;
        }

        // Sepete ürün ekleme
        [HttpPost("add")]
        public async Task<IActionResult> AddCartItem([FromBody] CartItemEntity cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("Cart item is null.");
            }

            await _cartItemRepo.AddAsync(cartItem);
            return Ok(cartItem);
        }

        // Kullanıcının tüm sepet ürünlerini getirme
        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetCartItems(int userId)
        {
            var cartItems = await _cartItemRepo.GetAll().Where(ci => ci.UserId == userId).ToListAsync();
            return Ok(cartItems);
        }

        // Sepet ürününü güncelleme
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItemEntity cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("Cart item is null.");
            }

            var updatedCartItem = await _cartItemRepo.UpdateAsync(cartItem);
            return Ok(updatedCartItem);
        }

        // Sepet ürününü silme
        [HttpDelete("{cartItemId:int}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var cartItem = await _cartItemRepo.GetByIdAsync(cartItemId);
            if (cartItem == null)
            {
                return NotFound();
            }

            await _cartItemRepo.DeleteAsync(cartItemId);
            return NoContent();
        }
    }
}

