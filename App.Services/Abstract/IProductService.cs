using App.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IProductService
    {
        Task<Result<ProductDto>> GetProductByIdAsync(int productId);
        Task<Result<List<ProductDto>>> GetAllProductsAsync();
        Task<Result<ProductDto>> CreateProductAsync(ProductDto productDto);
        Task<Result<ProductDto>> UpdateProductAsync(int productId, ProductDto productDto);
        Task<Result> DeleteProductAsync(int productId);
    }

}
