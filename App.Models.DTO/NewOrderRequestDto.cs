using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO
{
    // NewOrderRequestDto.cs
    namespace App.Models.DTO
    {
        public class NewOrderRequestDto
        {
            public List<OrderItemDto> Items { get; set; }
            public string DeliveryAddress { get; set; }
        }
    }

    // OrderItemDto.cs
    namespace App.Models.DTO
    {
        public class OrderItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }

    // NewOrderResponseDto.cs
    namespace App.Models.DTO
    {
        public class NewOrderResponseDto
        {
            public int OrderId { get; set; }
        }
    }

    // OrderDetailsDto.cs
    namespace App.Models.DTO
    {
        public class OrderDetailsDto
        {
            public int OrderId { get; set; }
            public List<OrderItemDto> Items { get; set; }
            public decimal TotalPrice { get; set; }
            public string DeliveryAddress { get; set; }
        }
    }


}
