using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public byte Quantity { get; set; }
    }
}
