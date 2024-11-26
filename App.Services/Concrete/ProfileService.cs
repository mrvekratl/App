using App.Services.Abstract;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Models.DTO;

namespace App.Services.Concrete
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient client;

        public ProfileService(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("DataApi");
        }

        public async Task<Result<ProfileResponseDto>> GetProfileAsync(string jwt)
        {
            var response = await SendApiRequestAsync("api/profile", HttpMethod.Get, jwt);
            return await ProcessResponseAsync<ProfileResponseDto>(response);
        }

        public async Task<Result> UpdateProfileAsync(string jwt, UpdateProfileRequestDto updateProfileRequest)
        {
            var response = await SendApiRequestAsync("api/profile", HttpMethod.Put, jwt, updateProfileRequest);
            return await ProcessResponseAsync(response);
        }

        // Diğer servisler için yardımcı metodlar burada olabilir
    }

}
