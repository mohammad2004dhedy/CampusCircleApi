namespace CampusCircleApi.Models
{
    public class PostLike
    {
        public int PostId{set;get;}
        public int UserId{set;get;}
        public User User{set;get;}
        public Post Post{set;get;}
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}