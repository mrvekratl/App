using App.Models.DTO;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest);
        Task<Result<RegisterResponseDto>> RegisterAsync(RegisterRequestDto registerRequest);
        Task<Result> LogoutAsync(string jwt);
    }
}
