using System.ComponentModel.DataAnnotations;
using CampusCircleApi.Enums;

namespace CampusCircleApi.Dtos
{
    public class UpdateUserProfileDto
    {
        [Required, StringLength(30, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required, StringLength(30, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public College College { get; set; }
    }
}
