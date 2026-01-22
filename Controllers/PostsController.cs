using CampusCircleApi.Data;
using CampusCircleApi.Dtos;
using CampusCircleApi.Models;
using Microsoft.AspNetCore.Mvc;
using CampusCircleApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusCircleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly CampusContext _context;

        public PostsController(CampusContext context)
        {
            _context = context;
        }

        // POST: /api/posts
[HttpPost]
public async Task<IActionResult> CreatePost([FromBody] CreatePostDto postDto)
{
    var user = await _context.Users.FindAsync(postDto.UserId);
    if (user == null) return NotFound("User not found");

    
    bool hasContent = !string.IsNullOrWhiteSpace(postDto.Content);
    bool hasMedia = postDto.MediaType != MediaType.None && !string.IsNullOrWhiteSpace(postDto.MediaPath);

   
    if (!hasContent && !hasMedia)
    {
        return BadRequest("Post must have content or media");
    }

    if (postDto.MediaType != MediaType.None && string.IsNullOrWhiteSpace(postDto.MediaPath))
    {
        return BadRequest("MediaPath is required when MediaType is not None");
    }

    var post = new Post
    {
        UserId = postDto.UserId,
        Content = hasContent ? postDto.Content : null,
        MediaType = postDto.MediaType,
        MediaPath = postDto.MediaPath,
        College = user.College
    };

    _context.Posts.Add(post);
    await _context.SaveChangesAsync();

    return Ok("post created");
}


        // DELETE: /api/posts/{postId}?userId=1
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost([FromRoute] int postId, [FromQuery] int userId)
        {
            if (userId <= 0) return BadRequest("Invalid userId");

            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return NotFound("Post not found");

            if (post.UserId != userId)
                return Forbid();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: /api/posts/feed?userId=1&scope=myCollege&page=1&pageSize=12
        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed(
            [FromQuery] int userId,
            [FromQuery] string scope = "myCollege",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {
            if (userId <= 0) return BadRequest("Invalid userId");
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 12;
            if (pageSize > 50) pageSize = 50;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found");

            var query = _context.Posts.AsNoTracking();

            if (!string.Equals(scope, "all", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(p => p.College == user.College);
            }

            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Content = p.Content,
                    MediaType = p.MediaType,
                    MediaPath = p.MediaPath,
                    College = p.College,
                    CreatedAt = p.CreatedAt,

                    User = new UserPostCommentDto
                    {
                        UserId = p.User.Id,
                        FullName = p.User.FirstName + " " + p.User.LastName,
                        ProfilePhotoPath = p.User.ProfilePhotoPath
                    },

                    LikeCount = p.PostLikes.Count(),
                    CommentCount = p.Comments.Count(),
                    IsLikedByCurrentUser = p.PostLikes.Any(pl => pl.UserId == userId),
                })
                .ToListAsync();

            return Ok(posts);
        }

        // GET: /api/posts/user/5?page=1&pageSize=12&currentUserId=1
[HttpGet("user/{userId}")]
public async Task<IActionResult> GetUserPosts(
    [FromRoute] int userId,
    [FromQuery] int currentUserId = 0,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 12)
{
    if (userId <= 0) return BadRequest("Invalid userId");
    if (page <= 0) page = 1;
    if (pageSize <= 0) pageSize = 12;
    if (pageSize > 50) pageSize = 50;

    var exists = await _context.Users.FindAsync(userId);
    if (exists == null) return NotFound("User not found");

    var posts = await _context.Posts
        .AsNoTracking()
        .Where(p => p.UserId == userId)
        .OrderByDescending(p => p.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(p => new PostDto
        {
            Id = p.Id,
            Content = p.Content,
            MediaType = p.MediaType,
            MediaPath = p.MediaPath,
            College = p.College,
            CreatedAt = p.CreatedAt,

            User = new UserPostCommentDto
            {
                UserId = p.User.Id,
                FullName = p.User.FirstName + " " + p.User.LastName,
                ProfilePhotoPath = p.User.ProfilePhotoPath
            },

            LikeCount = p.PostLikes.Count(),
            CommentCount = p.Comments.Count(),

            IsLikedByCurrentUser = currentUserId > 0
                ? p.PostLikes.Any(pl => pl.UserId == currentUserId)
                : false,
        })
        .ToListAsync();

    return Ok(posts);
}

    }
}
