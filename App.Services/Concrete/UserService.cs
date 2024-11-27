using App.Models.DTO;
using App.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly HttpClient _client;

        public UserService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("EcommerceApi");
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(int userId)
        {
            var response = await _client.GetAsync($"api/users/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return Result<UserDto>.Fail("Unable to fetch user.");
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            return Result<UserDto>.Success(user);
        }

        public async Task<Result<List<UserDto>>> GetAllUsersAsync()
        {
            var response = await _client.GetAsync("api/users");

            if (!response.IsSuccessStatusCode)
            {
                return Result<List<UserDto>>.Fail("Unable to fetch users.");
            }

            var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
            return Result<List<UserDto>>.Success(users);
        }

        public async Task<Result<UserDto>> CreateUserAsync(UserDto userDto)
        {
            var response = await _client.PostAsJsonAsync("api/users", userDto);

            if (!response.IsSuccessStatusCode)
            {
                return Result<UserDto>.Fail("Unable to create user.");
            }

            var createdUser = await response.Content.ReadFromJsonAsync<UserDto>();
            return Result<UserDto>.Success(createdUser);
        }

        public async Task<Result<UserDto>> UpdateUserAsync(int userId, UserDto userDto)
        {
            var response = await _client.PutAsJsonAsync($"api/users/{userId}", userDto);

            if (!response.IsSuccessStatusCode)
            {
                return Result<UserDto>.Fail("Unable to update user.");
            }

            var updatedUser = await response.Content.ReadFromJsonAsync<UserDto>();
            return Result<UserDto>.Success(updatedUser);
        }

        public async Task<Result> DeleteUserAsync(int userId)
        {
            var response = await _client.DeleteAsync($"api/users/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail("Unable to delete user.");
            }

            return Result.Success();
        }
    }

}
