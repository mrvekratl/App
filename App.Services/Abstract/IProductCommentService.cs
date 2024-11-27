using App.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IProductCommentService
    {
        Task<Result<List<ProductCommentDto>>> GetCommentsByProductIdAsync(int productId);
        Task<Result<ProductCommentDto>> CreateCommentAsync(ProductCommentDto commentDto);
        Task<Result<ProductCommentDto>> UpdateCommentAsync(int commentId, ProductCommentDto commentDto);
        Task<Result> DeleteCommentAsync(int commentId);
    }

}
