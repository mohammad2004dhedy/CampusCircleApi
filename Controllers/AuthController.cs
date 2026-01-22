using CampusCircleApi.Models;
using CampusCircleApi.Data;
using CampusCircleApi.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace CampusCircleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private CampusContext _context;
        public AuthController(CampusContext context)
        {
            _context=context;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto userDto)
        {
            var hasher=new PasswordHasher<User>();
            var u= await _context.Users.FirstOrDefaultAsync(u=>u.Email==userDto.Email.ToLower());
            if (u != null)
            {
                return BadRequest("user already exist");
            }
            var user=new User
            {
                FirstName=userDto.FirstName,
                LastName=userDto.LastName,
                Email=userDto.Email.ToLower(),
                BirthDate=userDto.BirthDate,
                College=userDto.College,
            };
            user.PasswordHash=hasher.HashPassword(user,userDto.Password);

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok("user added successfully ,go to login page to login to your account");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
            var hasher=new PasswordHasher<User>();
            var user=await _context.Users.FirstOrDefaultAsync(u=>u.Email==userDto.Email.ToLower());
            if (user==null)
            {
                return Unauthorized("Invalid email or password");
            }
            var result=hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                userDto.Password
            );
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password");
            }else
            {
                var res=new UserPostCommentDto
                {
                UserId=user.Id,
                FullName= $"{user.FirstName} {user.LastName}",
                ProfilePhotoPath=user.ProfilePhotoPath
                };
                return Ok(new
                {
                    hello=$"welcome {res.FullName}",
                    user=res
                });
            }
        }
    }
}