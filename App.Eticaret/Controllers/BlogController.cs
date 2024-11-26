using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    [Route("blog")]
    public class BlogController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BlogController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // TODO: seed data in api project
            var client = _httpClientFactory.CreateClient("Api.Data");
            var response = await client.GetAsync("blog");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }
            var content = await response.Content.ReadFromJsonAsync<List<BlogSummaryViewModel>>();
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var client = _httpClientFactory.CreateClient("Api.Data");
            var response = await client.GetAsync($"blog/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var content = await response.Content.ReadFromJsonAsync<BlogDetailViewModel>();
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }
    }
}