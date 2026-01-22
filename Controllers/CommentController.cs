using CampusCircleApi.Data;
using CampusCircleApi.Dtos;
using CampusCircleApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusCircleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly CampusContext _context;

        public CommentsController(CampusContext context)
        {
            _context = context;
        }

        // GET: /api/comments/post/10?page=1&pageSize=50
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetPostComments(
            [FromRoute] int postId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 50;
            if (pageSize > 100) pageSize = 100;

            var exists = await _context.Posts.FindAsync( postId);
            if (exists==null) return NotFound("Post not found");

            var comments = await _context.Comments
                .AsNoTracking()
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CommentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    Content = c.Content,
                    CommentDate = c.CommentDate,
                    User = new UserPostCommentDto
                    {
                        UserId = c.User.Id,
                        FullName = c.User.FirstName + " " + c.User.LastName,
                        ProfilePhotoPath = c.User.ProfilePhotoPath
                    }
                })
                .ToListAsync();

            return Ok(comments);
        }

        // POST: /api/comments
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists) return NotFound("User not found");

            var postExists = await _context.Posts.AnyAsync(p => p.Id == dto.PostId);
            if (!postExists) return NotFound("Post not found");

            var comment = new Comment
            {
                PostId = dto.PostId,
                UserId = dto.UserId,
                Content = dto.Content
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok("comment created");
        }

        // DELETE: /api/comments/{commentId}?userId=1
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int commentId, [FromQuery] int userId)
        {
                if (userId <= 0)
                return BadRequest("Invalid userId");

                var comment = await _context.Comments.FindAsync(commentId);
                if (comment == null)
                return NotFound("Comment not found");

                if (comment.UserId != userId)
                    return Forbid();

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return NoContent();
        }
    }
}
