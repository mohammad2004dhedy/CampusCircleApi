using CampusCircleApi.Enums;
namespace CampusCircleApi.Models
{
    public class User
    {
        public int Id{set;get;}
        public string FirstName{set;get;}
        public string LastName{set;get;}
        public string Email{set;get;}
        public string PasswordHash { get; set; }
        public DateTime BirthDate{set;get;}
        public College College{set;get;}
        public string ProfilePhotoPath { get; set; }="uploads/profile/default-profile.jpg";
        public DateTime CreatedAt{set;get;}=DateTime.UtcNow;
        public List<Post> Posts{set;get;}=new();
        public List<PostLike> UserLikes{set;get;}=new();
    }
}