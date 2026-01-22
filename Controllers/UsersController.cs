using CampusCircleApi.Data;
using CampusCircleApi.Dtos;
using CampusCircleApi.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusCircleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly CampusContext _context;
        private readonly IWebHostEnvironment _env;

        public UsersController(CampusContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /api/users/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    BirthDate = u.BirthDate,
                    College = u.College,
                    CreatedAt = u.CreatedAt,
                    ProfilePhotoPath = u.ProfilePhotoPath,
                    PostsCount = u.Posts.Count(),
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        // PUT: /api/users/5/profile?userId=5
        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromRoute] int id,
            [FromQuery] int userId,
            [FromBody] UpdateUserProfileDto dto)
        {
            if (userId != id) return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.BirthDate = dto.BirthDate;
            user.College = dto.College;

            await _context.SaveChangesAsync();
            return Ok("Profile updated");
        }

        // POST: /api/users/5/profile-photo?userId=5
        [HttpPost("{id}/profile-photo")]
        public async Task<IActionResult> UploadProfilePhoto(
            [FromRoute] int id,
            [FromQuery] int userId,
            [FromForm] IFormFile file)
        {
            if (userId != id) return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            const long maxBytes = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxBytes)
                return BadRequest("File too large (max 5MB)");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new HashSet<string> { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext))
                return BadRequest("Only jpg, jpeg, png, webp are allowed");

            var webRoot = _env.WebRootPath;

            var folder = Path.Combine(webRoot, "uploads", "profile");
            Directory.CreateDirectory(folder);

            var fileName = $"user-{id}-{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            await using (var stream = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"uploads/profile/{fileName}";
            user.ProfilePhotoPath = relativePath;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                profilePhotoPath = relativePath
            });
        }

        // DELETE: /api/users/5?userId=5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount([FromRoute] int id, [FromQuery] int userId)
        {
            if (userId != id) return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
