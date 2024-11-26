using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IProfileService
    {
        Task<Result<ProfileResponseDto>> GetProfileAsync(string jwt);
        Task<Result> RequestSellerApprovalAsync(string jwt);
    }
}
