using App.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetUserByIdAsync(int userId);
        Task<Result<List<UserDto>>> GetAllUsersAsync();
        Task<Result<UserDto>> CreateUserAsync(UserDto userDto);
        Task<Result<UserDto>> UpdateUserAsync(int userId, UserDto userDto);
        Task<Result> DeleteUserAsync(int userId);
    }

}
