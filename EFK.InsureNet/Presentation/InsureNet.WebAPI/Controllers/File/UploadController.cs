using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace InsureNet.WebAPI.Controllers.File
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController(IWebHostEnvironment _env) : ControllerBase
    {
        private readonly IWebHostEnvironment _env = _env;
        public class FileUploadRequest
        {
            [Required]
            public IFormFile File { get; set; }
        }

        [HttpPost("FileSaveUpload")]
        [Consumes("multipart/form-data")] // Swagger için mutlaka gerekli
        public async Task<IActionResult> FileSaveUpload([FromForm] FileUploadRequest request)
        {
            var file = request.File;

            if (file == null || file.Length == 0)
                return BadRequest("Dosya geçersiz.");

            var uploads = Path.Combine(_env.WebRootPath, "uploads", "logos");
            Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var fileUrl = $"/uploads/logos/{fileName}";
            return Ok(new { url = fileUrl });
        }

    }
}
