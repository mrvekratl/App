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
        private readonly HttpClient _httpClient;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AuthApi");
        }

        private async Task<HttpResponseMessage> SendApiRequestAsync(string apiRoute, HttpMethod method, string jwt = null, object payload = null)
        {
            var httpRequestMessage = new HttpRequestMessage(method, apiRoute);

            if (!string.IsNullOrEmpty(jwt))
            {
                httpRequestMessage.Headers.Add("Authorization", $"Bearer {jwt}");
            }

            if (payload != null)
            {
                httpRequestMessage.Content = JsonContent.Create(payload);
            }

            return await _httpClient.SendAsync(httpRequestMessage);
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest)
        {
            var response = await SendApiRequestAsync("api/auth/login", HttpMethod.Post, null, loginRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return Result<LoginResponseDto>.Fail($"Login failed: {errorMessage}");
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (loginResponse == null)
            {
                return Result<LoginResponseDto>.Fail("Login response is null.");
            }

            return Result<LoginResponseDto>.Success(loginResponse);
        }

        public async Task<Result<RegisterResponseDto>> RegisterAsync(RegisterRequestDto registerRequest)
        {
            var response = await SendApiRequestAsync("api/auth/register", HttpMethod.Post, null, registerRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return Result<RegisterResponseDto>.Fail($"Registration failed: {errorMessage}");
            }

            var registerResponse = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();
            if (registerResponse == null)
            {
                return Result<RegisterResponseDto>.Fail("Registration response is null.");
            }

            return Result<RegisterResponseDto>.Success(registerResponse);
        }

        public async Task<Result> LogoutAsync(string jwt)
        {
            var response = await SendApiRequestAsync("api/auth/logout", HttpMethod.Post, jwt);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return Result.Fail($"Logout failed: {errorMessage}");
            }

            return Result.Success();
        }
    }


}
