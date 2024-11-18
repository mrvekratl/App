using App.Api.Data.Models.ViewModels;
using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly DataRepository<ContactFormEntity> _cfRepo;

        public ContactController(DataRepository<ContactFormEntity> cfRepo)
        {
            _cfRepo = cfRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NewContactFormMessageViewModel newContactMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactMessageEntity = new ContactFormEntity
            {
                Name = newContactMessage.Name,
                Email = newContactMessage.Email,
                Message = newContactMessage.Message,
                SeenAt = null
            };

            await _cfRepo.AddAsync(contactMessageEntity);

            return Ok();
        }
    }

}
