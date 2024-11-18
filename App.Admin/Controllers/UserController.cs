using App.Admin.Models.ViewModels;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [Route("/users")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await _httpClient.GetFromJsonAsync<List<UserListItemViewModel>>("api/users");
            return View(users);
        }

        [Route("/users/{id:int}/approve")]
        [HttpGet]
        public async Task<IActionResult> ApproveSellerRequest([FromRoute] int id)
        {
            var response = await _httpClient.PostAsync($"api/users/{id}/approve-seller", null);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Seller request approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve seller request.";
            }
            return RedirectToAction(nameof(List));
        }

        [Route("/users/{id:int}/enable")]
        public async Task<IActionResult> Enable([FromRoute] int id)
        {
            var response = await _httpClient.PostAsync($"api/users/{id}/enable", null);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User enabled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to enable user.";
            }
            return RedirectToAction(nameof(List));
        }

        [Route("/users/{id:int}/disable")]
        public async Task<IActionResult> Disable([FromRoute] int id)
        {
            var response = await _httpClient.PostAsync($"api/users/{id}/disable", null);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User disabled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to disable user.";
            }
            return RedirectToAction(nameof(List));
        }
    }
}