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
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequest);
        Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequest);
        Task<Result> LogoutAsync(string jwt);
    }
}
