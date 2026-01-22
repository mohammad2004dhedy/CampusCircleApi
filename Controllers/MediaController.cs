using CampusCircleApi.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CampusCircleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public MediaController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // POST: /api/media/post
        // multipart/form-data:
        // - file: (required)
        [HttpPost("post")]
        public async Task<IActionResult> UploadPostMedia([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var webRoot = _env.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                return StatusCode(500, "WebRootPath is not configured. Ensure wwwroot exists.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            var imageExts = new HashSet<string> { ".jpg", ".jpeg", ".png", ".webp" };
            var videoExts = new HashSet<string> { ".mp4", ".webm" };

            MediaType mediaType;
            if (imageExts.Contains(ext)) mediaType = MediaType.Photo;
            else if (videoExts.Contains(ext)) mediaType = MediaType.Video;
            else return BadRequest("Unsupported file type. Allowed: jpg, jpeg, png, webp, mp4, webm");

            long maxBytes = mediaType == MediaType.Video
                ? 100L * 1024 * 1024   // 50MB
                : 5L * 1024 * 1024;   // 5MB

            if (file.Length > maxBytes)
                return BadRequest($"File too large (max {(mediaType == MediaType.Video ? "50MB" : "5MB")})");

            var relativeFolder = mediaType == MediaType.Photo
                ? "uploads/posts/images"
                : "uploads/posts/videos";

            var absoluteFolder = Path.Combine(webRoot, relativeFolder.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(absoluteFolder);

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var absolutePath = Path.Combine(absoluteFolder, fileName);

            await using (var stream = System.IO.File.Create(absolutePath))
            {
                await file.CopyToAsync(stream);
            }

            var mediaPath = $"{relativeFolder}/{fileName}";

            return Ok(new
            {
                mediaPath,
                mediaType = (int)mediaType
            });
        }
    }
}
