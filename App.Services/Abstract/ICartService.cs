using App.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface ICartService
    {
        Task<Result> AddToCartAsync(string jwt, CartItemDto cartItem);
        Task<Result> RemoveFromCartAsync(string jwt, int productId);
        Task<Result<CartDetailsDto>> GetCartDetailsAsync(string jwt);
    }
}
