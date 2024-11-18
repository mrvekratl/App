using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace App.Api.File.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        public FileController()
        {
            // Upload klasörünü oluştur
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        // Dosya Yükleme (POST)
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var filePath = Path.Combine(_uploadPath, file.FileName);

            // Dosyayı kaydetme
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return CreatedAtAction(nameof(Upload), new { fileName = file.FileName }, null);
        }

        // Dosya İndirme (GET)
        [HttpGet("download")]
        public IActionResult Download(string fileName)
        {
            var filePath = Path.Combine(_uploadPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileExtension = Path.GetExtension(fileName);
            string mimeType = "application/octet-stream"; // Varsayılan mime type

            // Dosya uzantısına göre mime type ayarlama
            if (fileExtension == ".jpg" || fileExtension == ".jpeg")
            {
                mimeType = "image/jpeg";
            }
            else if (fileExtension == ".png")
            {
                mimeType = "image/png";
            }
            else if (fileExtension == ".gif")
            {
                mimeType = "image/gif";
            }

            return File(fileBytes, mimeType, fileName);
        }
    }
}

