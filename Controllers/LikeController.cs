using CampusCircleApi.Data;
using CampusCircleApi.Dtos;
using CampusCircleApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusCircleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController : ControllerBase
    {
        private readonly CampusContext _context;

        public LikesController(CampusContext context)
        {
            _context = context;
        }

        // POST: /api/likes/toggle
        // Body: { "postId": 1, "userId": 2 }
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleLike([FromBody] CreateLikeDto dto)
        {

            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists) return NotFound("User not found");

            var postExists = await _context.Posts.AnyAsync(p => p.Id == dto.PostId);
            if (!postExists) return NotFound("Post not found");

            var like = await _context.PostLikes.FindAsync(dto.PostId, dto.UserId);

            bool likedNow;

            if (like != null)
            {
                //اذا في لايك من قبل ووجدته بالجدول روح امسحه وخلي القيمة الي رح ترجع فولس
                _context.PostLikes.Remove(like);
                likedNow = false;
            }
            else
            {
                // اذا فش لايك من قبل حطلي لايك جديد للبوست واليوزر
                _context.PostLikes.Add(new PostLike
                {
                    PostId = dto.PostId,
                    UserId = dto.UserId
                });
                likedNow = true;
            }

            await _context.SaveChangesAsync();

            var likeCount = await _context.PostLikes.CountAsync(pl => pl.PostId == dto.PostId);

            return Ok(new
            {
                liked = likedNow,
                likeCount
            });
        }

        // GET: /api/likes/user/2?limit=12
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetLastLikedPosts([FromRoute] int userId, [FromQuery] int limit = 12)
        {
            if (limit <= 0) limit = 12;
            if (limit > 50) limit = 50;

            var userExists = await _context.Users.AsNoTracking().AnyAsync(u => u.Id == userId);
            if (!userExists) return NotFound("User not found");

            var likedPosts = await _context.PostLikes
                .AsNoTracking()
                .Where(pl => pl.UserId == userId)
                .OrderByDescending(pl => pl.CreatedAt)
                .Take(limit)
                .Select(pl => new PostLikeDto
                {
                    LikedAt = pl.CreatedAt,
                    Post = new PostDto
                    {
                        Id = pl.Post.Id,
                        Content = pl.Post.Content,
                        MediaType = pl.Post.MediaType,
                        MediaPath = pl.Post.MediaPath,
                        College = pl.Post.College,
                        CreatedAt = pl.Post.CreatedAt,

                        User = new UserPostCommentDto
                        {
                            UserId = pl.Post.User.Id,
                            FullName = pl.Post.User.FirstName + " " + pl.Post.User.LastName,
                            ProfilePhotoPath = pl.Post.User.ProfilePhotoPath
                        },

                        LikeCount = pl.Post.PostLikes.Count(),
                        CommentCount = pl.Post.Comments.Count(),
                        IsLikedByCurrentUser = true
                    }
                })
                .ToListAsync();

            return Ok(likedPosts);
        }
    }
}
