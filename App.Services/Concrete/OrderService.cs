using App.Models.DTO.App.Models.DTO;
using App.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace App.Services.Concrete
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _client;

        public OrderService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("EcommerceApi");
        }

        public async Task<Result<NewOrderResponseDto>> PlaceOrderAsync(string jwt, NewOrderRequestDto newOrderRequest)
        {
            var response = await SendApiRequestAsync("api/order", HttpMethod.Post, jwt, newOrderRequest);

            if (!response.IsSuccessStatusCode)
            {
                return Result<NewOrderResponseDto>.Fail("Unable to place order.");
            }

            var newOrderResponse = await response.Content.ReadFromJsonAsync<NewOrderResponseDto>();
            return Result<NewOrderResponseDto>.Success(newOrderResponse);
        }

        public async Task<Result<List<OrderDetailsDto>>> GetOrdersAsync(string jwt)
        {
            var response = await SendApiRequestAsync("api/order/list", HttpMethod.Get, jwt);

            if (!response.IsSuccessStatusCode)
            {
                return Result<List<OrderDetailsDto>>.Fail("Unable to fetch orders.");
            }

            var orders = await response.Content.ReadFromJsonAsync<List<OrderDetailsDto>>();
            return Result<List<OrderDetailsDto>>.Success(orders);
        }

        public async Task<Result<OrderDetailsDto>> GetOrderDetailsAsync(string jwt, int orderId)
        {
            var response = await SendApiRequestAsync($"api/order/details/{orderId}", HttpMethod.Get, jwt);

            if (!response.IsSuccessStatusCode)
            {
                return Result<OrderDetailsDto>.Fail("Unable to fetch order details.");
            }

            var orderDetails = await response.Content.ReadFromJsonAsync<OrderDetailsDto>();
            return Result<OrderDetailsDto>.Success(orderDetails);
        }

        private async Task<HttpResponseMessage> SendApiRequestAsync(string apiRoute, HttpMethod method, string jwt, object payload = null)
        {
            var httpRequestMessage = new HttpRequestMessage(method, apiRoute)
            {
                Headers =
            {
                { HeaderNames.Authorization, $"Bearer {jwt}" }
            }
            };

            if (payload != null)
            {
                httpRequestMessage.Content = JsonContent.Create(payload);
            }

            return await _client.SendAsync(httpRequestMessage);
        }
    }


}
