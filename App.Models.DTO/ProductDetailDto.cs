using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO
{
    public class ProductDetailDto : ProductDto
    {
        public string ImageUrl { get; set; }
        public List<ProductCommentDto> Comments { get; set; }
    }

}
