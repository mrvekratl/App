using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO
{
    public class ProductResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductListResponseDto
    {
        public List<ProductResponseDto> Products { get; set; }
    }

    public class ProductDetailResponseDto
    {
        public ProductResponseDto Product { get; set; }
    }

    public class ProductCommentResponseDto
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public int Rating { get; set; }
    }

}
