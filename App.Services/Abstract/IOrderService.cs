using App.Models.DTO.App.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IOrderService
    {
        Task<Result<NewOrderResponseDto>> PlaceOrderAsync(string jwt, NewOrderRequestDto newOrderRequest);
        Task<Result<List<OrderDetailsDto>>> GetOrdersAsync(string jwt);
        Task<Result<OrderDetailsDto>> GetOrderDetailsAsync(string jwt, int orderId);
    }
}
