using App.Services.Abstract;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Models.DTO;
using System.Net.Http.Json;

namespace App.Services.Concrete
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _client;

        public ProfileService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("ProfileApi");
        }

        public async Task<Result<UserProfileDto>> GetProfileAsync(string jwt)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/profile");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Error("Failed to fetch profile");
            }
            var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
            return (Result<UserProfileDto>)Result.Success(profile);
        }

        public async Task<Result> UpdateProfileAsync(string jwt, UserProfileDto profileDto)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "api/profile")
            {
                Content = JsonContent.Create(profileDto)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode ? Result.Success() : Result.Error("Failed to update profile");
        }
    }


}
