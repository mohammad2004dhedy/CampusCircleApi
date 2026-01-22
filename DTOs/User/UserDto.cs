using CampusCircleApi.Enums;
using CampusCircleApi.Models;
namespace CampusCircleApi.Dtos
{
    public class UserDto
    {
        public int Id{set;get;}
        public string FirstName{set;get;}
        public string LastName{set;get;}
        public string Email{set;get;}
        public DateTime BirthDate{set;get;}
        public College College{set;get;}
        public DateTime CreatedAt{set;get;}
        public string ProfilePhotoPath{set;get;}
        public int PostsCount{set;get;}
    }
}