using System.ComponentModel.DataAnnotations;
namespace CampusCircleApi.Dtos
{
    public class CreateLikeDto
    {
        [Range(1, int.MaxValue)]
        public int PostId{set;get;}
        [Range(1, int.MaxValue)]
        public int UserId{set;get;}
    }
}