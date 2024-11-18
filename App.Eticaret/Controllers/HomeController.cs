using App.Data.Entities;
using App.Data.Infrastructure;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Eticaret.Controllers
{
    public class HomeController: BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/about-us")]
        public IActionResult AboutUs()
        {
            return View();
        }

        [HttpGet("/contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost("/contact")]
        public async Task<IActionResult> Contact([FromForm] NewContactFormMessageViewModel newContactMessage)
        {
            if (!ModelState.IsValid)
            {
                return View(newContactMessage);
            }

            var client = _httpClientFactory.CreateClient("ContactService");

            // Contact mesajýný API'ye gönderiyoruz
            var response = await client.PostAsJsonAsync("/api/contact", newContactMessage);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "There was an error sending your message.";
                return View(newContactMessage);
            }

            ViewBag.SuccessMessage = "Your message has been sent successfully.";
            return View();
        }

        [HttpGet("/product/list")]
        public async Task<IActionResult> Listing()
        {
            var client = _httpClientFactory.CreateClient("ProductService");

            // API'ye istek göndererek ürün listesini alýyoruz
            var products = await client.GetFromJsonAsync<List<ProductListingViewModel>>("/api/products");

            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            return View(products);
        }

        [HttpGet("/product/{productId:int}/details")]
        public async Task<IActionResult> ProductDetail([FromRoute] int productId)
        {
            var client = _httpClientFactory.CreateClient("ProductService");

            // API'ye istek göndererek ürün detaylarýný alýyoruz
            var product = await client.GetFromJsonAsync<HomeProductDetailViewModel>($"/api/products/{productId}");

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}