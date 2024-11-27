using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO
{
    public class CartDetailsDto
    {
        public List<CartItemDto> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
