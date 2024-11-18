using App.Api.Data.Models.ViewModels;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Data.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly DataRepository<OrderEntity> _orderRepo;
        private readonly DataRepository<OrderItemEntity> _orderItemRepo;
        private readonly DataRepository<CartItemEntity> _cartItemRepo;

        public OrderController(
            DataRepository<OrderEntity> orderRepo,
            DataRepository<OrderItemEntity> orderItemRepo,
            DataRepository<CartItemEntity> cartItemRepo)
        {
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _cartItemRepo = cartItemRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CheckoutViewModel model)
        {
            var userId = model.UserId;

            if (userId == null)
            {
                return BadRequest("User not logged in.");
            }

            var cartItems = await _cartItemRepo.GetAll()
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                return BadRequest("Cart is empty.");
            }

            var order = new OrderEntity
            {
                UserId = userId.Value,
                Address = model.Address,
                OrderCode = await CreateOrderCode(),
            };

            order = await _orderRepo.AddAsync(order);

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItemEntity
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price,
                };

                await _orderItemRepo.AddAsync(orderItem);
            }

            return CreatedAtAction(nameof(Details), new { orderCode = order.OrderCode }, order);
        }

        [HttpGet("{orderCode}/details")]
        public async Task<IActionResult> Details(string orderCode)
        {
            var order = await _orderRepo.GetAll()
                 .Where(o => o.OrderCode == orderCode)
                 .Select(o => new OrderDetailsViewModel
                 {
                     OrderCode = o.OrderCode,
                     CreatedAt = o.CreatedAt,
                     Address = o.Address,
                     Items = o.OrderItems.Select(oi => new OrderItemViewModel
                     {
                         ProductName = oi.Product.Name,
                         Quantity = oi.Quantity,
                         UnitPrice = oi.UnitPrice,
                     }).ToList()
                 })
                 .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        private async Task<string> CreateOrderCode()
        {
            var orderCode = Guid.NewGuid().ToString("n")[..16].ToUpperInvariant();
            while (await _orderRepo.GetAll().AnyAsync(x => x.OrderCode == orderCode))
            {
                orderCode = Guid.NewGuid().ToString("n")[..16].ToUpperInvariant();
            }

            return orderCode;
        }
    }
}

