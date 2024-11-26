using App.Models.DTO;
using App.Services.Abstract;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Http;
using Microsoft.Net.Http.Headers;


namespace App.Services.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient client;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("DataApi");
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequest)
        {
            var response = await SendApiRequestAsync("api/auth/register", HttpMethod.Post, null, registerRequest);
            return await ProcessResponseAsync<AuthResponseDto>(response);
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequest)
        {
            var response = await SendApiRequestAsync("api/auth/login", HttpMethod.Post, null, loginRequest);
            return await ProcessResponseAsync<AuthResponseDto>(response);
        }

        public async Task<Result> LogoutAsync(string jwt)
        {
            var response = await SendApiRequestAsync("api/auth/logout", HttpMethod.Post, jwt);
            var result = await ProcessResponseAsync<AuthResponseDto>(response);

            // AuthResponseDto'yu işlemek, sadece Result döndürmek için
            if (!result.IsSuccess)
            {
                return Result.Fail("Çıkış işlemi başarısız oldu.");
            }

            return Result.Success();
        }



        // JWT ile istek göndermek için yardımcı metot
        private async Task<HttpResponseMessage> SendApiRequestAsync(string apiRoute, HttpMethod method, string jwt, object payload = null)
        {
            var httpRequestMessage = new HttpRequestMessage(method, apiRoute)
            {
                Headers = { { HeaderNames.Authorization, $"Bearer {jwt}" } }
            };
            if (payload is not null)
            {
                httpRequestMessage.Content = JsonContent.Create(payload);
            }

            return await client.SendAsync(httpRequestMessage);
        }

        private async Task<Result<T>> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                // Erken dönüşler
                return Result.Fail<T>("Bir hata oluştu");
            }

            var data = await response.Content.ReadFromJsonAsync<T>();
            return Result.Success(data);
        }

    }

}
