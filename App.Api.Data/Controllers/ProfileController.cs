using App.Api.Data.Models.ViewModels;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace App.Data.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly DataRepository<UserEntity> _userRepo;
        private readonly DataRepository<OrderEntity> _orderRepo;
        private readonly DataRepository<ProductEntity> _productRepo;

        public ProfileController(DataRepository<UserEntity> userRepo, DataRepository<OrderEntity> orderRepo, DataRepository<ProductEntity> productRepo)
        {
            _userRepo = userRepo;
            _orderRepo = orderRepo;
            _productRepo = productRepo;
        }

        [HttpGet("details")]
        public async Task<IActionResult> Details([FromQuery] int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] ProfileDetailsViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var updatedUser = await _userRepo.UpdateAsync(new UserEntity
            {
                Id = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password // Assuming password handling is done securely
            });

            if (updatedUser == null)
            {
                return BadRequest("Failed to update profile.");
            }

            return Ok(updatedUser);
        }
       

        [HttpGet("my-orders")]
        public async Task<IActionResult> MyOrders([FromQuery] int userId)
        {
            var orders = await _orderRepo.GetByIdAsync(userId);

            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            return Ok(orders);
        }

        [HttpGet("my-products")]
        public async Task<IActionResult> MyProducts([FromQuery] int userId)
        {
            var products = await _productRepo.GetByIdAsync(userId);

            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }
    }

}
