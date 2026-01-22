using CampusCircleApi.Enums;

namespace CampusCircleApi.Models
{
    public class Post
    {
        public int Id{get;set;}
        public int UserId{set;get;}
        public string? Content{set;get;}
        public MediaType MediaType{set;get;}=MediaType.None;
        public string? MediaPath{set;get;}
        public User User{set;get;}
        public DateTime CreatedAt{set;get;}=DateTime.UtcNow;
        public College College{set;get;}
        public List<Comment> Comments{set;get;}=new();
        public List<PostLike> PostLikes{set;get;}=new();
    }
}