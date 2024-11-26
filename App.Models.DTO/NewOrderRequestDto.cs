using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO
{
    public class NewOrderRequestDto
    {
        public List<NewOrderItemDto> Items { get; set; }
        public string DeliveryAddress { get; set; }
    }

    public class NewOrderItemDto
    {
        public int ProductId { get; set; }
        public byte Quantity { get; set; }
    }

    public class NewOrderResponseDto
    {
        public int OrderId { get; set; }
    }

    public class MyOrdersResponseDto
    {
        public List<OrderDto> Orders { get; set; }
    }

    public class MyOrderDetailResponseDto
    {
        public OrderDetailDto OrderDetail { get; set; }
    }

}
