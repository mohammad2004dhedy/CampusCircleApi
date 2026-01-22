using CampusCircleApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusCircleApi.Dtos
{
    public class CreatePostDto
    {
        [Range(1, int.MaxValue)]
        public int UserId { set; get; }

        [StringLength(1000)]
        public string? Content { set; get; }

        [Required]
        public MediaType MediaType { set; get; } = MediaType.None;

        [StringLength(300)]
        public string? MediaPath { set; get; }
    }
}
