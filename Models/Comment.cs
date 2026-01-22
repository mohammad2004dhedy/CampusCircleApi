namespace CampusCircleApi.Models
{
    public class Comment
    {
        public int Id{set;get;}
        public int PostId{set;get;}
        public int UserId{set;get;} 
        public string Content{get;set;}
        public DateTime CommentDate{set;get;}=DateTime.UtcNow;
        public User User{set;get;}
        public Post Post{set;get;}
    }
}