using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Data.Entities;
using App.Data.Infrastructure;
using App.Api.Data.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace App.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        private readonly DataRepository<UserEntity> _userRepo;

        public UserController(DataRepository<UserEntity> userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepo.GetAll()
                .Where(u => u.RoleId != 1) // Exclude admins
                .Select(u => new UserListItemViewModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role.Name,
                    Enabled = u.Enabled,
                    HasSellerRequest = u.HasSellerRequest
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("{id:int}/approve-seller")]
        public async Task<IActionResult> ApproveSellerRequest(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            if (!user.HasSellerRequest)
                return BadRequest("No seller request found for this user.");

            user.HasSellerRequest = false;
            user.RoleId = 2; // Seller role
            await _userRepo.UpdateAsync(user);

            return Ok("Seller request approved.");
        }

        [HttpPost("{id:int}/enable")]
        public async Task<IActionResult> EnableUser(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Enabled = true;
            await _userRepo.UpdateAsync(user);

            return Ok("User enabled.");
        }

        [HttpPost("{id:int}/disable")]
        public async Task<IActionResult> DisableUser(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Enabled = false;
            await _userRepo.UpdateAsync(user);

            return Ok("User disabled.");
        }
    }
}

